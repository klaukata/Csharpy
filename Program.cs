using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        // connection string for the SQL Server database
        string connectionString =
                        "Data Source=DESKTOP-SATMIQ7\\SQLEXPRESS;" +
                        "Initial Catalog=charpy;" +
                        "Integrated Security=True;" +
                        "TrustServerCertificate=true";
        SqlConnection connection = new SqlConnection(connectionString);

        Database db = new Database(connection);
        AuthService auth = new AuthService(db);
        UserController userController = new UserController(auth, db);

        // starting a program
        userController.Start();
    }
}