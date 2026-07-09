import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import ModalT from "../components/ModalT";
import ".././css/HomePage.css";

const API_URL = import.meta.env.VITE_API_URL as string;

interface Transaction{
  transactionId: number;
  userId: number;
  familyId: number;
  cashValue: number;
  typeOfTransaction: string;
  description: string;
}

interface FamilyUser{
  id: number;
  name: string;
}

interface TransactionFormData{
  [key: string]: string;
}


function HomePage() {
  const navigate = useNavigate();
  const [error, SetError] = useState("");
  const [loading, SetLoading] = useState(false);
  const [userAge, setUserAge] = useState<number | null>(null);
  const [familyUsers, SetFamilyUsers] = useState<FamilyUser[]>([]);
  const [transactions, SetTransactions] = useState<Transaction[]>([]);
  const [selectedUserId, SetSelectedUserId] = useState<number | null>(null);

  const fetchTransactions = async (targetUserId: number) => {
    SetSelectedUserId(targetUserId);
    SetLoading(true);
    SetError("");
    SetTransactions([]);
    const token = localStorage.getItem("jwt_token");

    try {
      const response = await fetch(`${API_URL}/api/user/transaction/${targetUserId}`, {
        method: "GET",
        headers: {
            "content-type": "application/json",
            "Authorization": `Bearer ${token}`
        }
      });

      if(!response.ok){
        if(response.status == 403) throw new Error("Você não tem permissão para ver estes dados")
        if(response.status == 401){
          localStorage.removeItem("jwt_token");
          navigate("/");
          throw new Error("Sessão expirada");
        }
      }

      const data: Transaction[] = await response.json();
      SetTransactions(data);

    } catch (err){
        if(err instanceof Error) SetError(err.message);
    } finally{
        SetLoading(false);
    }
  }

  const fetchFamilyTransactions = async () => {
    SetSelectedUserId(null); // Usa-se null para indicar que estamos vendo a família
    SetLoading(true);
    SetError("");
    SetTransactions([]);
    const token = localStorage.getItem("jwt_token");

    try {
      const response = await fetch(`${API_URL}/api/transaction/family`, {
        method: "GET",
        headers: { "Authorization": `Bearer ${token}` }
      });

      if (!response.ok) throw new Error("Erro ao buscar transações da família.");

      const data: Transaction[] = await response.json();
      SetTransactions(data);

    } catch (err) {
        if(err instanceof Error) SetError(err.message);
    } finally {
        SetLoading(false);
    }};

  const fetchFamilyUsers = async (token: string) => {
    try {
        const response = await fetch(`${API_URL}/api/user/family`, {
            method: "GET",
            headers: { "Authorization": `Bearer ${token}` }
        });
        if (response.ok) {
            const data = await response.json();
            SetFamilyUsers(data);
            
            // Seleciona automaticamente o primeiro usuário da lista ao carregar a página
            if (data.length > 0 && selectedUserId === null) {
                fetchTransactions(data[0].id);
            }
        }
    } catch (err) {
        console.error("Erro ao buscar familiares", err);
    }
  };

  const handleCreateTransaction = async (data: TransactionFormData) => {
    const token = localStorage.getItem("jwt_token");
    const valorNumerico = parseFloat(data["Valor (R$)"]);

    const response = await fetch(`${API_URL}/api/transaction/create`, {
      method: "POST",
      headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
      },
      body: JSON.stringify({
          CashValue: valorNumerico,
          TypeOfTransaction: data["Tipo"],
          Description: data["Descrição"]
      })
    });

    if (response.ok) {
        alert("Transação registrada!");
        if (selectedUserId) fetchTransactions(selectedUserId); // Atualiza a tabela
    } else {
        alert("Erro ao registrar transação. Verifique os dados.");
    }
  };

  const handleDeleteTransaction = async (transactionId: number) => {
    const token = localStorage.getItem("jwt_token");
    const response = await fetch(`${API_URL}/api/transaction/${transactionId}`, {
        method: "DELETE",
        headers: { "Authorization": `Bearer ${token}` }
    });

    if (response.ok) {
        alert("Transação removida.");
        if (selectedUserId) fetchTransactions(selectedUserId); // Atualiza a tabela
    } else {
        alert("Erro ao remover transação.");
    }
  };

  const handleDeleteUser = async (userName: string) => {
    const confirmar = window.confirm(`Cuidado! Tem certeza que deseja deletar o usuário "${userName}"?`);
    if (!confirmar) return;

    const token = localStorage.getItem("jwt_token");
    const response = await fetch(`${API_URL}/api/user/${userName}`, {
        method: "DELETE",
        headers: { "Authorization": `Bearer ${token}` }
    });

    if (response.ok) {
        alert("Usuário deletado com sucesso.");
        // Atualiza a lista da barra lateral para remover o nome deletado
        fetchFamilyUsers(token!); 
    } else {
        alert("Você não tem permissão para deletar este usuário.");
    }
  };

  const handleLogout = () => {
    localStorage.removeItem("jwt_token");
    navigate("/");
  };

  useEffect(() => {
    const token = localStorage.getItem("jwt_token");
    const age = localStorage.getItem("user_age");
    if(!token){
      navigate("/");
      return;
    }

    if(age){
        setUserAge(parseInt(age));
    }

    fetchFamilyUsers(token);
  }, [navigate]);
  
  const totalValue = transactions.reduce((acc, current) => {
    const isDespesa = current.typeOfTransaction.toLocaleLowerCase() === "despesa";
    return isDespesa ? acc - current.cashValue : acc + current.cashValue; // Se for despesa, retorna subtraindo, do contrário soma.
  }, 0)

  return (
    <div id="homepage-layout">
      <aside id="lateral-navigator">
        <div className="logo-area">FinDom</div>
        <ul id="users-list">
          {familyUsers.map((user) => (
              <li key={user.id} className="user-list-item">
                  <button
                      className="reload-transactions-button" 
                      onClick={() => fetchTransactions(user.id)}
                  >
                      {user.name}
                  </button>
                  <button
                      className="delete-user-button"
                      onClick={() => handleDeleteUser(user.name)}
                      title="Deletar usuário"
                  >
                      X
                  </button>
              </li>
            ))}
            <li className="user-list-item" style={{ backgroundColor: selectedUserId === null ? '#34495e' : 'transparent' }}>
              <button
                className="reload-transactions-button" 
                onClick={fetchFamilyTransactions}
                style={{ fontWeight: 'bold' }}
              >
                Visão da Família
              </button>
          </li>
        </ul>
      </aside>

      <main id="main-content">
          <header id="top-header">
            <h2 style={{color:"black"}}>Visão Geral de Transações</h2>
            <div className="header-actions">
                <button 
                    className="header-btn"
                    onClick={() => {
                        const token = localStorage.getItem("jwt_token");
                        if(token) fetchFamilyUsers(token);
                    }} 
                >
                    Atualizar
                </button>

                <ModalT<TransactionFormData> 
                    fields={["Valor (R$)", "Tipo", "Descrição"]} 
                    secretFields={[]} 
                    onModalSubmit={handleCreateTransaction}
                    mountElement="root"
                    userAge={userAge}
                >
                    + Nova Transação
                </ModalT>

                <button onClick={handleLogout} className="header-btn logout-btn">
                    Sair
                </button>
            </div>
          </header> 

          <div id="table-component">
              {loading && <p>Carregando dados...</p>}
              {error && <p className="error-msg">{error}</p>}

              {!loading && !error && transactions.length === 0 && (
                  <p className="empty-state">Nenhuma transação registrada para este usuário.</p>
              )}

              {!loading && transactions.length > 0 && (
                  <table className="transaction-table">
                      <thead>
                          <tr>
                              <th>ID</th>
                              <th>Descrição</th>
                              <th>Tipo</th>
                              <th>Valor (R$)</th>
                              <th>Ações</th>
                          </tr>
                      </thead>
                      <tbody>
                          {transactions.map((t) => {
                              return(<tr key={t.transactionId}>
                                  <td>{t.transactionId}</td>
                                  <td>{t.description}</td>
                                  <td>{t.typeOfTransaction}</td>
                                  <td style={{ color: t.typeOfTransaction.trim().toLowerCase() === 'despesa' ? 'red' : 'green', fontWeight: 'bold' }}>
                                    {t.typeOfTransaction.trim().toLowerCase() === 'despesa' ? '-' : '+'} {t.cashValue}
                                  </td>
                                  <td>
                                    <button 
                                      className="delete-transaction-button" 
                                      onClick={() => {handleDeleteTransaction(t.transactionId)}}
                                    >
                                      Deletar
                                    </button>
                                  </td>
                              </tr>
                              );
                          })}
                      </tbody>
                      <tfoot>
                          <tr>
                              <td colSpan={3} style={{ textAlign: 'right', fontWeight: 'bold', border: '1px solid whitesmoke', padding: '12px' }}>
                                  TOTAL:
                              </td>
                              <td style={{ fontWeight: 'bold', border: '1px solid whitesmoke', padding: '12px' }} className={totalValue < 0 ? 'negative-val' : 'positive-val'}>
                                  {totalValue.toFixed(2)}
                              </td>
                              <td style={{ border: '1px solid whitesmoke' }}></td>
                          </tr>
                      </tfoot>
                  </table>
              )}
          </div>
      </main>
    </div>
  )
}

export default HomePage