// pages/api/connexion_traitement.tsx
import { fetchDataFromAPI } from './function';
import { NextApiRequest, NextApiResponse } from 'next';
import { AuthentificationPerso } from '../../authentification'; //  côté serveur
export default async function handler(req: NextApiRequest, res: NextApiResponse) {
    if (req.method === 'POST') {
      try {
        // On traite les données du formulaire Body
        const formData = req.body;
        const cookieValues = await AuthentificationPerso.recup_session_user(req);
  
        // On fait la requête à l'API Privé en utilisant la clé API
        const responseAPI = await fetchDataFromAPI('/auth/post/validate_registration', 'POST', formData, cookieValues);
  
        res.status(200).json(responseAPI);
      } catch (error) {
        console.error('Erreur lors de la requête à l\'API externe', error);
        res.status(500).json({ error: 'Erreur lors de la requête à l\'API externe' });
      }
    } else {
      res.status(405).json({ error: 'Méthode non autorisée' });
    }
} 