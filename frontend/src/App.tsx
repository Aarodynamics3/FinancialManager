import './App.css';
import { Link } from './Components/Link/Link';
import { Fetch } from './Components/Fetch/Fetch';
import { useState } from 'react';
import { PlaidCredentails } from './Interfaces/PlaidCredentials';

function App() {
  const [plaidCredentials, setPlaidCredentials] = useState<PlaidCredentails>({} as PlaidCredentails);

  return (
    <div className="App">
      <Link setPlaidCredentials={setPlaidCredentials} />
      <Fetch plaidCredentials={plaidCredentials} />
    </div>
  );
}

export default App;
