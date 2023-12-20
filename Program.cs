using System;
using System.Threading.Tasks;
using Npgsql;

class Program
{
    static string connectionString = "Host=localhost;Port=5455;Username=postgres;Password=postgres;Database=sys23m";

    static async Task Main()
    {
        // Initialize the database
        await InitializeDatabase();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Välkommen till Holidaymaker!");
            Console.WriteLine("1. Skapa bokning");
            Console.WriteLine("2. Registrera ny kund");
            Console.WriteLine("3. Avboka rum");
            Console.WriteLine("4. Sök lediga rum");
            Console.WriteLine("5. Ändra bokningsdetaljer");
            Console.WriteLine("6. Avsluta");

            Console.Write("Vänligen välj ett alternativ (1-6): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await CreateBooking();
                    break;

                case "2":
                    await RegisterNewCustomer();
                    break;

                case "3":
                    await CancelBooking();
                    break;

                case "4":
                    await SearchAvailableRooms();
                    break;

                case "5":
                    await ModifyBookingDetails();
                    break;

                case "6":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Ogiltigt val. Försök igen.");
                    break;
            }

            Console.WriteLine("Tryck på valfri tangent för att fortsätta...");
            Console.ReadKey();
        }
    }

    static async Task InitializeDatabase()
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            // Create Customer table
            using (var cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS Customer(" +
                "First_name VARCHAR(50), " +
                "Last_name VARCHAR(50), " +
                "Mail VARCHAR(100), " +
                "Phone_number INTEGER, " +
                "Date_of_Birth DATE, " +
                "Customer_id SERIAL PRIMARY KEY)", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Create Accommodation table
            using (var cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS Accommodation(" +
                "Accommodation_id SERIAL PRIMARY KEY, " +
                "Location VARCHAR(150), " +
                "Size INTEGER, " +
                "Available VARCHAR(200), " +
                "Description TEXT)", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Create Booking table
            using (var cmd = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS Booking(" +
                "Booking_id SERIAL PRIMARY KEY, " +
                "Date DATE, " +
                "Number_of_persons INTEGER, " +
                "Additional_services VARCHAR(150), " +
                "Price DECIMAL(10, 2), " +
                "Customer_id INTEGER REFERENCES Customer(Customer_id), " +
                "Accommodation_id INTEGER REFERENCES Accommodation(Accommodation_id))", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Insert data into Customer
            using (var cmd = new NpgsqlCommand("INSERT INTO Customer" +
                "(First_name, Last_name, Mail, Phone_number, Date_of_Birth) " +
                "VALUES('John', 'Doe', 'john.doe@email.com', 12345678, '1990-05-15')," +
                "('Jane', 'Smith', 'jane.smith@email.com', 98765432, '1985-08-22');", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Insert data into Accommodation 
            using (var cmd = new NpgsqlCommand("INSERT INTO Accommodation" +
            " (Location, Size, Available, Description)" +
            "VALUES" +
            "('City Center', 100, 'Yes', 'Luxurious hotel in the heart of the city')," +
            "('City Center', 120, 'Yes', 'Spacious suite in the city center')," +
            "('City Center', 140, 'Yes', 'Elegant room with city views')," +

            "('Beachfront Resort', 160, 'Yes', 'Beachfront suite with ocean views')," +
            "('Beachfront Resort', 180, 'Yes', 'Spacious room overlooking the beach')," +
            "('Beachfront Resort', 200, 'Yes', 'Private villa by the sea')," +

            "('Rural Retreat', 120, 'Yes', 'Cozy cottage in the countryside')," +
            "('Rural Retreat', 140, 'Yes', 'Scenic cabin with nature views')," +
            "('Rural Retreat', 160, 'Yes', 'Rustic lodge surrounded by greenery')," +

            "('Mountain Chalet', 90, 'Yes', 'Ski-in, ski-out chalet with mountain views')," +
            "('Mountain Chalet', 110, 'Yes', 'Quaint mountain cabin for nature lovers')," +
            "('Mountain Chalet', 130, 'Yes', 'Secluded chalet with panoramic vistas')," +

            "('Seaside Escape', 100, 'Yes', 'Seaside room with a view')," +
            "('Seaside Escape', 120, 'Yes', 'Beachfront bungalow with tropical vibes')," +
            "('Seaside Escape', 140, 'Yes', 'Oceanfront suite with modern amenities')," +

            "('Lakeview Lodge', 80, 'Yes', 'Lakeside cabin with a cozy fireplace')," +
            "('Lakeview Lodge', 100, 'Yes', 'Tranquil retreat by the lake')," +
            "('Lakeview Lodge', 120, 'Yes', 'Charming cottage with lake views')," +

            // Add values for other locations...

            "('Urban Oasis', 110, 'Yes', 'Urban loft in the heart of the city')," +
            "('Urban Oasis', 130, 'Yes', 'Modern apartment with city skyline views')," +
            "('Urban Oasis', 150, 'Yes', 'Chic penthouse with luxury amenities')," +

            "('Historic Haven', 70, 'Yes', 'Historic inn with period charm')," +
            "('Historic Haven', 90, 'Yes', 'Stately mansion with a rich history')," +
            "('Historic Haven', 110, 'Yes', 'Antique-filled bed and breakfast')," +

            // Add values for other locations...

            "('Desert Retreat', 120, 'Yes', 'Desert oasis with stunning sunset views')," +
            "('Desert Retreat', 140, 'Yes', 'Spacious desert villa with modern comforts')," +
            "('Desert Retreat', 160, 'Yes', 'Rustic cabin in the midst of the desert')," +

            "('Forest Getaway', 150, 'Yes', 'Cabin in the heart of the forest')," +
            "('Forest Getaway', 170, 'Yes', 'Treehouse retreat surrounded by nature')," +
            "('Forest Getaway', 190, 'Yes', 'Cozy cottage with forest views')," +

            // Add values for other locations...

            "('Island Paradise', 200, 'Yes', 'Exclusive beach house on a tropical island')," +
            "('Island Paradise', 220, 'Yes', 'Private villa with oceanfront access')," +
            "('Island Paradise', 240, 'Yes', 'Luxurious resort on a secluded island');", connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
    static async Task RegisterNewCustomer()
    {
        Console.WriteLine("Registrera ny kund.");

        Console.Write("Ange förnamn: ");
        string firstName = Console.ReadLine();

        Console.Write("Ange efternamn: ");
        string lastName = Console.ReadLine();

        Console.Write("Ange e-post: ");
        string email = Console.ReadLine();

        Console.Write("Ange telefonnummer: ");
        int phoneNumber = int.Parse(Console.ReadLine());

        Console.Write("Ange födelsedatum (ÅÅÅÅ-MM-DD): ");
        DateTime dateOfBirth = DateTime.Parse(Console.ReadLine());

        // Call the RegisterNewCustomer method
        int customerId = await RegisterNewCustomer(firstName, lastName, email, phoneNumber, dateOfBirth);

        if (customerId != -1)
        {
            Console.WriteLine($"Kund registrerad med ID: {customerId}");
        }
        else
        {
            Console.WriteLine("Misslyckades att registrera kund. Försök igen.");
        }
    }

    static async Task<int> RegisterNewCustomer(string firstName, string lastName, string email, int phoneNumber, DateTime dateOfBirth)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Customer (First_name, Last_name, Mail, Phone_number, Date_of_Birth) VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth) RETURNING Customer_id", connection))
            {
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                command.Parameters.AddWithValue("@DateOfBirth", dateOfBirth);

                var customerId = await command.ExecuteScalarAsync();

                return customerId != null ? Convert.ToInt32(customerId) : -1;
            }
        }
    }


    static async Task<int> GetCustomerId(string firstName, string lastName)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand("SELECT Customer_id FROM Customer WHERE First_name = @FirstName AND Last_name = @LastName", connection))
            {
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);

                var result = await command.ExecuteScalarAsync();

                return result != null ? Convert.ToInt32(result) : -1;
            }
        }
    }


    static async Task CreateBooking()
    {
        Console.WriteLine("Skapa en ny bokning.");

        Console.Write("Ange kundens förnamn: ");
        string firstName = Console.ReadLine();

        Console.Write("Ange kundens efternamn: ");
        string lastName = Console.ReadLine();

        int customerId = await GetCustomerId(firstName, lastName);

        if (customerId == 0)
        {
            Console.WriteLine("Kunden existerar inte. Du kan endast skapa en bokning för befintliga kunder.");
            return;
        }

        Console.Write("Ange antal personer: ");
        int numberOfPeople = int.Parse(Console.ReadLine());

        Console.Write("Ange boendelokation: ");
        string location = Console.ReadLine();

        int accommodationId = await GetAccommodationId(location);

        if (accommodationId == 0)
        {
            Console.WriteLine("Boendelokationen existerar inte. Kontrollera stavningen och försök igen.");
            return;
        }

        Console.Write("Ange pris: ");
        decimal price = decimal.Parse(Console.ReadLine());

        Console.Write("Ange extra tjänster: ");
        string additionalServices = Console.ReadLine();

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand("INSERT INTO Booking (Customer_id, Number_of_persons, Date, Accommodation_id, Price, Additional_services) VALUES (@CustomerId, @NumberOfPeople, current_date, @AccommodationId, @Price, @AdditionalServices)", connection))
            {
                command.Parameters.AddWithValue("@CustomerId", customerId);
                command.Parameters.AddWithValue("@NumberOfPeople", numberOfPeople);
                command.Parameters.AddWithValue("@AccommodationId", accommodationId);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@AdditionalServices", additionalServices);

                await command.ExecuteNonQueryAsync();
            }

            Console.WriteLine("Bokning skapad!");
        }
    }


    static async Task<int> GetAccommodationId(string location)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand("SELECT Accommodation_id FROM Accommodation WHERE Location = @Location", connection))
            {
                command.Parameters.AddWithValue("@Location", location);

                var result = await command.ExecuteScalarAsync();

                return result != null ? Convert.ToInt32(result) : 0;
            }
        }
    }

    static async Task CancelBooking()
    {
        Console.WriteLine("Avboka en bokning.");
        Console.Write("Ange bokningsID för avbokning: ");
        int bookingId = int.Parse(Console.ReadLine());

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand("DELETE FROM Booking WHERE Booking_id = @BookingID", connection))
            {
                command.Parameters.AddWithValue("@BookingID", bookingId);

                int rowsAffected = await command.ExecuteNonQueryAsync();

                if (rowsAffected > 0)
                {
                    Console.WriteLine("Bokning avbruten!");
                }
                else
                {
                    Console.WriteLine("Misslyckades att avboka bokning. Kontrollera bokningsID och försök igen.");
                }
            }
        }
    }

    static async Task SearchAvailableRooms()
    {
        Console.WriteLine("Sök lediga rum.");
        Console.Write("Ange antal personer: ");
        int numberOfPeople = int.Parse(Console.ReadLine());

        Console.Write("Ange boendelokation: ");
        string location = Console.ReadLine();

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM Accommodation WHERE Size >= @NumberOfPeople AND Location = @Location AND Available = 'Yes'", connection))
            {
                command.Parameters.AddWithValue("@NumberOfPeople", numberOfPeople);
                command.Parameters.AddWithValue("@Location", location);

                using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("Lediga rum:");
                        while (await reader.ReadAsync())
                        {
                            Console.WriteLine($"ID: {reader["Accommodation_id"]}, Location: {reader["Location"]}, Size: {reader["Size"]}, Description: {reader["Description"]}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Inga lediga rum hittades för angivna kriterier.");
                    }
                }
            }
        }
    }




    static async Task ModifyBookingDetails()
    {
        Console.WriteLine("Ändra bokningsdetaljer.");

        Console.Write("Ange bokningsID för ändring: ");
        int bookingId = int.Parse(Console.ReadLine());

        // Check if the booking exists
        if (await BookingExists(bookingId))
        {
            Console.Write("Ange nytt antal personer: ");
            int numberOfPeople = int.Parse(Console.ReadLine());

            Console.Write("Ange nytt pris: ");
            decimal price = decimal.Parse(Console.ReadLine());

            Console.Write("Ange nya extra tjänster: ");
            string additionalServices = Console.ReadLine();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (NpgsqlCommand command = new NpgsqlCommand("UPDATE Booking SET Number_of_persons = @NumberOfPeople, Price = @Price, Additional_services = @AdditionalServices WHERE Booking_id = @BookingId", connection))
                {
                    command.Parameters.AddWithValue("@NumberOfPeople", numberOfPeople);
                    command.Parameters.AddWithValue("@Price", price);
                    command.Parameters.AddWithValue("@AdditionalServices", additionalServices);
                    command.Parameters.AddWithValue("@BookingId", bookingId);

                    int rowsAffected = await command.ExecuteNonQueryAsync();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Bokningsdetaljer ändrade!");
                    }
                    else
                    {
                        Console.WriteLine("Misslyckades att ändra bokningsdetaljer. Kontrollera bokningsID och försök igen.");
                    }
                }
            }
        }
        else
        {
            Console.WriteLine("Bokningen med angivet ID existerar inte.");
        }
    }

    static async Task<bool> BookingExists(int bookingId)
    {
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (NpgsqlCommand command = new NpgsqlCommand("SELECT COUNT(*) FROM Booking WHERE Booking_id = @BookingId", connection))
            {
                command.Parameters.AddWithValue("@BookingId", bookingId);

                int count = Convert.ToInt32(await command.ExecuteScalarAsync());

                return count > 0;
            }
        }
    }
}
