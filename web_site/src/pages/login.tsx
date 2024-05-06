import 'tailwindcss/tailwind.css';
import React, { useEffect } from 'react';
import { useRouter } from 'next/navigation'
import { AuthStatus } from "@/Hook/type";
import { UseAuth } from "@/Hook/UseAuth";
import Loader1 from "../app/components/Loader1";
import Image from 'next/image';
import googleLogo from '../../public/assets/images/logo_google.png';
import Head from 'next/head';



export default function LoginGoogle() {



    const { status, GetAllUsersToMatch, LoginGoogle } = UseAuth();
    const router = useRouter()



    useEffect(() => {
        GetAllUsersToMatch();
        let timer: NodeJS.Timeout | undefined;
        if (status === AuthStatus.Authenticated) {
            timer = setTimeout(() => {
                router.push('/welcome');
            }, 5000);
        }
        return () => clearTimeout(timer);
    }, [status, GetAllUsersToMatch, router]);




    if (status === AuthStatus.Unauthenticated)
    {
        return (
            <div className="bg-pink-200 flex flex-col items-center justify-center h-screen sm:p-6">
                <Head>
                    <title>AmourConnect</title>
                    <link rel="icon" href="/assets/images/amour_connect_logo.jpg" />
                </Head>
                <h1 className="text-3xl font-bold mb-8 text-center sm:text-4xl text-black">Connexion uniquement avec Google❤</h1>
                <div className="flex items-center mb-4 sm:mb-6">
                    <Image src={googleLogo} alt="Logo Google" className="h-6 w-6 mr-2 sm:h-8 sm:w-8" />
                    <button
                        type="button"
                        className="px-6 py-3 bg-red-500 text-white font-medium rounded hover:bg-red-600 focus:outline-none sm:px-8 sm:py-4"
                        onClick={LoginGoogle}
                    >
                        Se connecter avec Google
                    </button>
                </div>
            </div>
        );
    }



    return (
            <Loader1 />
    );
}