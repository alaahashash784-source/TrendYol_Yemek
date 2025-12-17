<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Security.Cryptography" %>
<!DOCTYPE html>
<html>
<head>
    <title>Create Restaurant Admins</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body class="bg-light">
<script runat="server">
    System.Collections.Generic.List<string> results = new System.Collections.Generic.List<string>();
    
    protected void Page_Load(object sender, EventArgs e)
    {
        CreateAllAdmins();
    }
    
    void CreateAllAdmins()
    {
        string connectionString = "Data Source=localhost;Initial Catalog=TrendYolYemekDB;Integrated Security=True;";
        string defaultPassword = "admin123";
        
        // Restaurant admins data
        var admins = new[] {
            new { RestoranId = 1, Email = "kebap@admin.com", Ad = "Kebap" },
            new { RestoranId = 2, Email = "pizza@admin.com", Ad = "Pizza" },
            new { RestoranId = 3, Email = "burger@admin.com", Ad = "Burger" },
            new { RestoranId = 4, Email = "deniz@admin.com", Ad = "Deniz" },
            new { RestoranId = 5, Email = "asya@admin.com", Ad = "Asya" },
            new { RestoranId = 6, Email = "tatli@admin.com", Ad = "Tatli" }
        };
        
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                foreach (var admin in admins)
                {
                    // Check if email exists
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Musteriler WHERE Email = @Email", conn);
                    checkCmd.Parameters.AddWithValue("@Email", admin.Email);
                    int count = (int)checkCmd.ExecuteScalar();
                    
                    string hashedPassword = HashPassword(defaultPassword);
                    
                    if (count > 0)
                    {
                        // Update existing
                        SqlCommand updateCmd = new SqlCommand(
                            "UPDATE Musteriler SET Sifre = @Sifre, IsRestoranAdmin = 1, RestoranId = @RestoranId WHERE Email = @Email", conn);
                        updateCmd.Parameters.AddWithValue("@Sifre", hashedPassword);
                        updateCmd.Parameters.AddWithValue("@RestoranId", admin.RestoranId);
                        updateCmd.Parameters.AddWithValue("@Email", admin.Email);
                        updateCmd.ExecuteNonQuery();
                        results.Add("UPDATED: " + admin.Email);
                    }
                    else
                    {
                        // Create new
                        SqlCommand insertCmd = new SqlCommand(
                            @"INSERT INTO Musteriler (Ad, Soyad, Email, Sifre, KayitTarihi, IsAdmin, IsRestoranAdmin, RestoranId) 
                              VALUES (@Ad, N'Yönetici', @Email, @Sifre, GETDATE(), 0, 1, @RestoranId)", conn);
                        insertCmd.Parameters.AddWithValue("@Ad", admin.Ad);
                        insertCmd.Parameters.AddWithValue("@Email", admin.Email);
                        insertCmd.Parameters.AddWithValue("@Sifre", hashedPassword);
                        insertCmd.Parameters.AddWithValue("@RestoranId", admin.RestoranId);
                        insertCmd.ExecuteNonQuery();
                        results.Add("CREATED: " + admin.Email + " / " + defaultPassword);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            results.Add("ERROR: " + ex.Message);
        }
    }
    
    string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(salt);
        }
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000))
        {
            byte[] hash = pbkdf2.GetBytes(32);
            byte[] hashBytes = new byte[48];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 32);
            return Convert.ToBase64String(hashBytes);
        }
    }
</script>

<div class="container py-5">
    <div class="card shadow">
        <div class="card-header bg-success text-white">
            <h3><i class="fas fa-check"></i> Restaurant Admins Created!</h3>
        </div>
        <div class="card-body">
            <h4>Results:</h4>
            <ul class="list-group mb-4">
                <% foreach (string r in results) { %>
                    <li class="list-group-item"><%= r %></li>
                <% } %>
            </ul>
            
            <h4>Login Credentials:</h4>
            <table class="table table-bordered">
                <thead class="table-dark">
                    <tr><th>Restaurant</th><th>Email</th><th>Password</th></tr>
                </thead>
                <tbody>
                    <tr><td>Turk Kebap Evi</td><td><strong>kebap@admin.com</strong></td><td>admin123</td></tr>
                    <tr><td>Pizza Cenneti</td><td><strong>pizza@admin.com</strong></td><td>admin123</td></tr>
                    <tr><td>Burger Bolgesi</td><td><strong>burger@admin.com</strong></td><td>admin123</td></tr>
                    <tr><td>Deniz Lezzetleri</td><td><strong>deniz@admin.com</strong></td><td>admin123</td></tr>
                    <tr><td>Asya Tatlari</td><td><strong>asya@admin.com</strong></td><td>admin123</td></tr>
                    <tr><td>Tatli Ruyalar</td><td><strong>tatli@admin.com</strong></td><td>admin123</td></tr>
                </tbody>
            </table>
            
            <div class="text-center mt-4">
                <a href="/Account/Login" class="btn btn-primary btn-lg">Go to Login Page</a>
            </div>
        </div>
    </div>
</div>
</body>
</html>
