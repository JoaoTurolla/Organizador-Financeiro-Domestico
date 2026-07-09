import React from 'react'
import ModalT from '../components/ModalT';
import { useNavigate } from 'react-router-dom'
import '.././css/LoginPage.css'

interface modalData{
    [key: string]: string;
}

const API_URL = import.meta.env.VITE_API_URL as string;

async function sendCredentialsForLogin(password: string, username: string): Promise<boolean> {
    const response = await fetch(`${API_URL}/api/login/request`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Password: password, UserName: username })
        })

    if (!response.ok) {
        throw new Error('Usuário ou senha inválidos. Tente novamente.');
    }

    const data = await response.json();
    const token = data.token || data.Token;
    const userAge = data.age || data.Age;
    if(token){
        localStorage.setItem('jwt_token', token);
        localStorage.setItem('user_age', userAge);
    }

    return true;
}

function LoginPage() {
    const navigate = useNavigate();
    const[erro, setErro] = React.useState('');

    const handleSubmit = async (e: React.SyntheticEvent<HTMLFormElement>) => {
        e.preventDefault();

        const formData = new FormData(e.currentTarget);

        const psswrd = (formData.get('password') as string)  || '';
        const username = (formData.get('username') as string) || '';

        try {
            const success = await sendCredentialsForLogin(psswrd, username);

            if(success){
                navigate('/home');
            }
        } catch (error) {
            if(error instanceof Error){
                setErro(error.message);
            }
        }
    }

    const handleData = async (data: modalData) => {
        const username = data["Usuário"];
        const userAge = data["Idade"];
        const password = data["Senha"];

        const response = await fetch(`${API_URL}/api/create`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({UserName: username, UserAge: parseInt(userAge), Password: password })
            } );
        if(!response.ok){
            alert("Falha ao criar usuário, o nome pode já estar em uso.");
            throw new Error("Dados inválidos");
        } else {
            alert("Usuário criado com sucesso! Faça o login.");
        }
    }

    return (
        <div id='login-page'>
            <form onSubmit={handleSubmit} className="login-form">
                <h2 id="login-title">Login</h2>
                <input type="text" name='username' placeholder="Nome de Usuário" className="login-input" />
                <input type="password" name='password' placeholder="Senha" className="login-input" />
                <button type="submit" className="login-button">Enviar</button>
                {erro && <p className="login-error">{erro}</p>}
            </form>
            
            <ModalT<modalData> 
                fields={["Usuário", "Idade"]} 
                secretFields={["Senha"]} 
                onModalSubmit={handleData}
                mountElement='root'
            >
                Criar usuário
            </ModalT>
        </div>
    )
}

export default LoginPage