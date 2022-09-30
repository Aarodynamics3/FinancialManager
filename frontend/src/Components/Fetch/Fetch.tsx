import React, { useState } from 'react';
import axios from 'axios';
import { PlaidCredentails } from '../../Interfaces/PlaidCredentials';
import { FetchCommand } from '../../Interfaces/FetchCommand';

interface FetchProps {
    plaidCredentials: PlaidCredentails
}

export const Fetch: React.FC<FetchProps> = ({plaidCredentials}) => {
    const serverUrl = "https://localhost:7081";
    const [transactions, setTransactions] = useState<string[]>([]);

    const getTransactions = async () => {
        const request: FetchCommand = {
            accessToken: plaidCredentials.accessToken
        }
        console.log(request);
        const response = axios.get(serverUrl + "/api/fetch/transactions", {params: request});
        const result = await response;
        setTransactions(result.data);
        console.log(result.data)
    }

    const transactionsList = transactions.map((transaction: string, id: number) => {
        return <li key={id}>{transaction}</li>
    });

    return (
        <div>
            <button onClick={() => getTransactions()}>
                Get Transactions
            </button>
            <ul>
                {transactionsList}
            </ul>
        </div>
    )
}