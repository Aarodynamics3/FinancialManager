import axios from 'axios';
import React, { useCallback, useEffect, useState } from 'react';
import { usePlaidLink, PlaidLinkOnSuccess } from 'react-plaid-link';
import { ExchangePublicTokenCommand } from '../../Interfaces/ExchangePublicTokenCommand';
import { PlaidCredentails } from '../../Interfaces/PlaidCredentials';

interface LinkProps {
    setPlaidCredentials: React.Dispatch<React.SetStateAction<PlaidCredentails>>
}

export const Link: React.FC<LinkProps> = ({setPlaidCredentials}) => {
    const [token, setToken] = useState<string | null>(null);
    
    const serverUrl = "https://localhost:7081";

    useEffect(() => {
        const createLinkToken = async () => {
            const response = await fetch(serverUrl + "/api/link", { method: 'GET' });
            const link_token = await response.text();
            setToken(link_token);
            console.log(link_token);
        };
        createLinkToken();
    }, []);

    const onSuccess = useCallback<PlaidLinkOnSuccess>((publicToken, _metadata) => {
        // Send public token to server.
        postPublicToken(publicToken);
    }, []);

    const postPublicToken = async (publicToken: string) => {
        const request: ExchangePublicTokenCommand = {
            token: publicToken
        }
        const response = axios.post(serverUrl + "/api/link", request);
        const result = await response;
        setPlaidCredentials(result.data as PlaidCredentails);
        console.log(result.data)
    }

    const { open, ready } = usePlaidLink({
        token,
        onSuccess,
        // onEvent
        // onExit
    });

    return (
        <div>
            <button onClick={() => open()} disabled={!ready}>
                Connect a bank account
            </button>
        </div>
    );
}