using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/* operations for authentication logic
    - registering
    - logging in
*/

class AuthService
{
    private readonly Database Database;
    public AuthService(Database db)
    {
        Database = db;
    }

    public User? Login()
    {
        while (true)
        {
            try
            {
                Console.Write("Enter your email: ");
                string? email = Console.ReadLine() ?? throw new ArgumentNullException("Email cannot be null.");

                if (!Database.IsEmailRegistered(email))
                {
                    Console.WriteLine("Provided email is not registered yet.");
                    Console.Write("Would you like to exit? (Y/N): ");
                    string? choice = Console.ReadLine()?.ToUpper();

                    if (choice == "Y")
                    {
                        return null; // Exit login process
                    }
                    else if (choice != "N")
                    {
                        Console.WriteLine("Invalid choice. Please try again.");
                    }
                    continue; // Restart the loop
                }

                Console.Write("Enter your password: ");
                string? password = Console.ReadLine() ?? throw new ArgumentNullException("Password cannot be null.");

                User user = new User(email, password);
                if (Database.ValidateUser(user))
                {
                    Console.WriteLine("Login successful!");
                    return user;
                }
                else
                {
                    Console.WriteLine("Incorrect password.");
                    return null; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during login: {ex.Message}");
            }
        }
    }

    public void Register()
    {
        try
        {
            // validate email
            string emailRegexPattern = "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$";
            string? email = "";
            while (true)
            {
                Console.Write("Enter your email: ");
                email = Console.ReadLine() ?? throw new ArgumentNullException("Provided email cannot be null");
                if (!Regex.IsMatch(email, emailRegexPattern))
                {
                    Console.WriteLine("Email does not match the required format (letters/numbers/hyphens/underscores/dots@letters.letters).  Please try again.");
                }
                else if (Database.IsEmailRegistered(email))
                {
                    Console.WriteLine("This email is already registered. Please try again.");
                }
                else break;
            }

            string passwordRegexPattern = "^(?=.*?[0-9]).{5,}$";
            string? password_1, password_2;

            // validate passworD
            while (true)
            {
                Console.Write("Enter your password: ");
                password_1 = Console.ReadLine() ?? throw new ArgumentNullException("Provided password cannot be null.");
                if (!Regex.IsMatch(password_1, passwordRegexPattern))
                {
                    Console.WriteLine("Password has to have at least one digit and be at least 5 characters long. Please try again.");
                }
                else break;
            }
            // asking to retype a password and checks if provided passwords match
            while (true)
            {
                Console.Write("Reenter your password: ");
                password_2 = Console.ReadLine() ?? throw new ArgumentNullException("Provided retyped password cannot be null.");
                if (password_1 != password_2)
                {
                    Console.WriteLine("Provided passwords do not match. Please try again.");
                }
                else break;
            }

            // Create and add user to the database
            User userToInsert = new User(email, password_1);
            Database.AddUser(userToInsert);
            Console.WriteLine("Registration successful!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during registration: {ex.Message}");
        }
    }
}