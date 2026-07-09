# Organizador-Financeiro-Domestico

Requisitos:
  - Git instalado (git clone)
  - Node.js (especificamente pelo npm)
  - Dotnet sdk (dotnet run e dotnet restore)

# Instruções de uso

1. Clone o repositório com: git clone https://github.com/JoaoTurolla/Organizador-Financeiro-Domestico
2. Atualize as variáveis nas duas .env.example e remova o .example do nome (Uma estará na pasta do cliente e outra do servidor)
3. Na pasta raíz do cliente (mesma pasta em que a env do cliente se encontra) use o comando "npm i" para instalar as dependências
4. Na pasta raíz do servidor (mesma pasta em que a env do servidor se encontra) use o comando "dotnet restore" pelo mesmo motivo
5. Inicie o servidor com "dotnet run" ainda na pasta raíz, certifique-se que o endereço em que o servidor está é o mesmo colocado na env do cliente, e então vá para a pasta raíz do cliente e use o comando "npm run dev", agora certifique-se que o endereço que o cliente está é o mesmo colocado na env do servidor.
6. Abra o navegador no endereço do cliente e está tudo pronto

# Pontos importantes
  Na pasta do servidor será criado uma pasta Data/ que conterá dois arquivos .json; Note que no arquivo users.json todos os usuários terão o campo { "FamilyId": 1, }, este campo pode ser alterado, levando em consideração que:

  1. Todas as transações desse usuário previamente existentes a essa mudança necessitarão a mesma mudança para que a tabela da família funcione corretamente;
  2. O valor -1 é reservado para indicar que não há família atribuída

O campo { "RoleLevel": 3 } também pode ser alterado, embora apenas o valor 1 tenha funcionalidades específicas (privilégio de admin que permite deletar outros usuários)  
  Qualquer mudança diretamente nos arquivos salvos requer o desligamento do servidor préviamente.
