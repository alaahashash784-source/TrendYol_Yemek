<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Security.Cryptography" %>
<%@ Import Namespace="System.Text" %>
<!DOCTYPE html>
<html>
<head>
    <title>Admin Setup & Auto Login</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
</head>
<body class="bg-light">
<div class="container py-5">
    <div class="card shadow mx-auto" style="max-width: 600px;">
        <div class="card-header bg-primary text-white">
            <h4 class="mb-0">Admin Account Setup</h4>
        </div>
        <div class="card-body">
<%
    try
    {
        string connectionString = "Data Source=localhost;Initial Catalog=TrendYolYemekDB;Integrated Security=True;";
        
        // Hash password function (same as SecurityHelper)
        Func<string, string> hashPassword = (password) => {
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
        };
        
        string hashedPassword = hashPassword("admin123");
        
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            
            // Check if admin exists
            SqlCommand checkCmd = new SqlCommand("SELECT MusteriId FROM Musteriler WHERE Email = 'admin@trendyol.com'", conn);
            object result = checkCmd.ExecuteScalar();
            
            int adminId;
            
            if (result == null)
            {
                // Create admin account
                SqlCommand insertCmd = new SqlCommand(@"
                    INSERT INTO Musteriler (Ad, Soyad, Email, Sifre, Telefon, Adres, KayitTarihi, IsAdmin) 
                    OUTPUT INSERTED.MusteriId
                    VALUES ('Admin', 'User', 'admin@trendyol.com', @Sifre, '5551234567', 'Admin Address', GETDATE(), 1)", conn);
                insertCmd.Parameters.AddWithValue("@Sifre", hashedPassword);
                adminId = (int)insertCmd.ExecuteScalar();
                
                Response.Write("<div class='alert alert-success'>");
                Response.Write("<h5>✅ Admin hesabi olusturuldu!</h5>");
                Response.Write("</div>");
            }
            else
            {
                adminId = (int)result;
                
                // Update password and ensure IsAdmin is true
                SqlCommand updateCmd = new SqlCommand("UPDATE Musteriler SET Sifre = @Sifre, IsAdmin = 1 WHERE Email = 'admin@trendyol.com'", conn);
                updateCmd.Parameters.AddWithValue("@Sifre", hashedPassword);
                updateCmd.ExecuteNonQuery();
                
                Response.Write("<div class='alert alert-info'>");
                Response.Write("<h5>✅ Admin hesabi guncellendi!</h5>");
                Response.Write("</div>");
            }
            
            // Get admin info
            SqlCommand getCmd = new SqlCommand("SELECT Ad, Soyad FROM Musteriler WHERE MusteriId = @Id", conn);
            getCmd.Parameters.AddWithValue("@Id", adminId);
            SqlDataReader reader = getCmd.ExecuteReader();
            string fullName = "Admin User";
            if (reader.Read())
            {
                fullName = reader["Ad"] + " " + reader["Soyad"];
            }
            reader.Close();
            
            // Set session - AUTO LOGIN
            Session["MusteriId"] = adminId;
            Session["MusteriAd"] = fullName;
            Session["IsAdmin"] = true;
            
            Response.Write("<div class='alert alert-success'>");
            Response.Write("<h5>🎉 Otomatik giris yapildi!</h5>");
            Response.Write("<p>Admin olarak giris yaptiniz. Artik tum sayfalara erisebilirsiniz.</p>");
            Response.Write("</div>");
            
            Response.Write("<div class='bg-light p-3 rounded mb-3'>");
            Response.Write("<h6>Admin Bilgileri:</h6>");
            Response.Write("<p><strong>Email:</strong> admin@trendyol.com</p>");
            Response.Write("<p><strong>Sifre:</strong> admin123</p>");
            Response.Write("<p><strong>Session MusteriId:</strong> " + adminId + "</p>");
            Response.Write("<p><strong>Session IsAdmin:</strong> True</p>");
            Response.Write("</div>");
        }
    }
    catch (Exception ex)
    {
        Response.Write("<div class='alert alert-danger'>Hata: " + ex.Message + "<br><br>" + ex.StackTrace + "</div>");
    }
%>
            <h5 class="mt-4">Simdi yapabilecekleriniz:</h5>
            <div class="d-grid gap-2 mt-3">
                <a href="/Admin" class="btn btn-primary btn-lg">🎛️ Admin Paneli</a>
                <a href="/Restaurant/Create" class="btn btn-success">🏪 Yeni Restoran Ekle</a>
                <a href="/Food/Create" class="btn btn-warning">🍕 Yeni Yemek Ekle</a>
                <a href="/" class="btn btn-outline-secondary">🏠 Ana Sayfa</a>
            </div>
        </div>
    </div>
</div>
</body>
</html>
