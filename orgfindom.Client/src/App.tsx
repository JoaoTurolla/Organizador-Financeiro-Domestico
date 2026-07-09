// import { useQuery } from '@tanstack/react-query'
import { BrowserRouter, Routes, Route } from 'react-router-dom'
import LoginPage from './views/LoginPage.tsx'
import HomePage from './views/HomePage.tsx'

// interface Message {
//   texto: string;
// }

// const fetchMessage = async (): Promise<Message>  => {
//   const response = await fetch('Aqui tinha um url');
  
//   if(!response.ok){
//     throw new Error('Network response was not ok');
//   }

//   return response.json();
// }

function App() {
  // const { data, error, isLoading, isError } = useQuery({queryKey: ['messageBackend'], queryFn: fetchMessage});

  // if(isLoading){
  //   return <div>Loading...</div>
  // }

  // if(isError){
  //   return <div>Error: {error instanceof Error ? error.message : 'Unknown error'}</div>
  // }

  // return (
  //   <p>{data?.texto}</p>
  // );

  return(
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LoginPage />} />
        <Route path="/home" element={<HomePage />} />
      </Routes>
    </BrowserRouter>
  )



}
export default App
