﻿using server_api.Data;
using server_api.Interfaces;
using server_api.Models;
using server_api.Utils;
using Microsoft.EntityFrameworkCore;
using server_api.Dto.AppLayerDto;
using server_api.Dto.GetDto;

namespace server_api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApiDbContext _context;

        public UserRepository(ApiDbContext context) 
        { 
            _context = context;
        }

        public ICollection<GetUserDto> GetUsersToMatch(User data_user_now_connect)
        {
            return _context.User
            .Where(u =>
                u.city.ToLower() == data_user_now_connect.city.ToLower() &&
                u.sex == (data_user_now_connect.sex == "M" ? "F" : "M") &&
                u.date_of_birth >= (data_user_now_connect.sex == "F" ?
                    data_user_now_connect.date_of_birth.AddYears(-10) :
                    data_user_now_connect.date_of_birth.AddYears(-1)) &&
                u.date_of_birth <= (data_user_now_connect.sex == "M" ?
                    data_user_now_connect.date_of_birth.AddYears(10) :
                    data_user_now_connect.date_of_birth.AddYears(1)) &&
            !_context.RequestFriends.Any(r =>
                ((r.IdUserIssuer == u.Id_User && r.Id_UserReceiver == data_user_now_connect.Id_User) ||
                (r.Id_UserReceiver == u.Id_User && r.IdUserIssuer == data_user_now_connect.Id_User)) &&
                r.Status == RequestStatus.Accepted))
            .Select(u => new GetUserDto
            {
                Id_User = u.Id_User,
                Pseudo = u.Pseudo,
                Profile_picture = u.Profile_picture,
                city = u.city,
                sex = u.sex,
                date_of_birth = u.date_of_birth,
            })
            .ToList();
        }



        public int? GetUserIdWithGoogleId(string EmailGoogle, string userIdGoogle)
        {
            return _context.User
                .Where(u => u.EmailGoogle == EmailGoogle && u.userIdGoogle == userIdGoogle)
                .Select(u => u.Id_User)
                .FirstOrDefault();
        }



        public int? CreateUser(string userIdGoogle, string EmailGoogle, DateTime? date_of_birth, string sex, string Pseudo, string city)
        {
            var user = new User
            {
                userIdGoogle = userIdGoogle,
                EmailGoogle = EmailGoogle,
                date_of_birth = date_of_birth.HasValue ? date_of_birth.Value.ToUniversalTime() : DateTime.MinValue,
                sex = sex,
                Pseudo = Pseudo,
                city = city,
                account_created_at = DateTime.Now.ToUniversalTime(),
            };

            _context.User.Add(user);
            var rowsAffected = _context.SaveChanges();

            if (rowsAffected > 0)
            {
                return user.Id_User;
            }
            return null;
        }



        public ALSessionUserDto UpdateSessionUser(int Id_User)
        {
            string newSessionToken;
            DateTime expirationDate;

            do
            {
                newSessionToken = MessUtils.GeneratePassword(64);
                expirationDate = DateTime.UtcNow.AddDays(7);

            } while (_context.User.Any(u => u.token_session_user == newSessionToken));


            var user = _context.User.FirstOrDefault(u => u.Id_User == Id_User);

            if (user != null)
            {
                user.token_session_user = newSessionToken;
                user.date_token_session_expiration = expirationDate;

                _context.SaveChanges();
            }

            return new ALSessionUserDto
            {
                token_session_user = newSessionToken,
                date_token_session_expiration = expirationDate
            };
        }



        public bool GetUserByPseudo(string Pseudo)
        {
            return _context.User.Any(u => u.Pseudo.ToLower() == Pseudo.ToLower());
        }



        public User GetUserWithCookie(string token_session_user)
        {
            return _context.User.FirstOrDefault(u => u.token_session_user == token_session_user);
        }

        public async Task<User> GetUserWithCookieAsync(string token_session_user)
        {
            return await _context.User.FirstOrDefaultAsync(u => u.token_session_user == token_session_user);
        }

        public async Task<bool> UpdateUser(int Id_User, User user)
        {
            User existingUser = await _context.User.FirstOrDefaultAsync(u => u.Id_User == Id_User);

            if (existingUser == null)
            {
                return false;
            }

            existingUser.date_of_birth = user.date_of_birth.ToUniversalTime();
            existingUser.sex = user.sex;
            existingUser.Profile_picture = user.Profile_picture;
            existingUser.city = user.city;

            _context.Entry(existingUser).State = EntityState.Modified;
            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }



        public User GetUserByIdUser(int Id_User)
        {
            return _context.User
                .Where(u => u.Id_User == Id_User)
                .FirstOrDefault();
        }
    }
}