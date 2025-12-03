<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Security.Cryptography" %>
<!DOCTYPE html>
<html>
<head>
    <title>Admin Login</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
    <style>
        body {
            background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%);
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .admin-card {
            background: rgba(255, 255, 255, 0.95);
            border-radius: 20px;
            box-shadow: 0 25px 50px rgba(0,0,0,0.3);
            overflow: hidden;
            max-width: 420px;
            width: 100%;
        }
        .admin-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 40px 30px;
            text-align: center;
            color: white;
        }
        .admin-header i {
            font-size: 60px;
            margin-bottom: 15px;
            opacity: 0.9;
        }
        .admin-body {
            padding: 40px 30px;
        }
        .form-control {
            border-radius: 10px;
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
        }
        .form-control:focus {
            border-color: #667eea;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
        }
        .btn-admin {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border: none;
            border-radius: 10px;
            padding: 12px;
            font-weight: 600;
            letter-spacing: 1px;
        }
        .btn-admin:hover {
            transform: translateY(-2px);
            box-shadow: 0 5px 20px rgba(102, 126, 234, 0.4);
        }
        .input-group-text {
            background: #f8f9fa;
            border: 2px solid #e0e0e0;
            border-right: none;
            border-radius: 10px 0 0 10px;
        }
        .input-group .form-control {
            border-left: none;
            border-radius: 0 10px 10px 0;
        }
    </style>
</head>
<body>
<div class="admin-card">
    <div class="admin-header">
        <i class="fas fa-user-shield"></i>
        <h3 class="mb-0">Admin Panel</h3>
        <p class="mb-0 opacity-75">Yetkili giris</p>
    </div>
    <div class="admin-body">
<%
    string errorMessage = "";
    string successMessage = "";
    
    // Only allow specific admin credentials
    string ADMIN_EMAIL = "admin@trendyol.com";
    string ADMIN_PASSWORD = "admin123";
    
    if (Request.HttpMethod == "POST")
    {
        string email = Request.Form["email"];
        string password = Request.Form["password"];
        
        // Check if credentials match the hardcoded admin
        if (email == ADMIN_EMAIL && password == ADMIN_PASSWORD)
        {
            try
            {
                string connectionString = "Data Source=localhost;Initial Catalog=TrendYolYemekDB;Integrated Security=True;";
                
                // Hash password function
                Func<string, string> hashPassword = (pwd) => {
                    byte[] salt = new byte[16];
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(salt);
                    }
                    using (var pbkdf2 = new Rfc2898DeriveBytes(pwd, salt, 10000))
                    {
                        byte[] hash = pbkdf2.GetBytes(32);
                        byte[] hashBytes = new byte[48];
                        Array.Copy(salt, 0, hashBytes, 0, 16);
                        Array.Copy(hash, 0, hashBytes, 16, 32);
                        return Convert.ToBase64String(hashBytes);
                    }
                };
                
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    
                    // Check if admin exists
                    SqlCommand checkCmd = new SqlCommand("SELECT MusteriId, Ad, Soyad FROM Musteriler WHERE Email = @Email", conn);
                    checkCmd.Parameters.AddWithValue("@Email", ADMIN_EMAIL);
                    SqlDataReader reader = checkCmd.ExecuteReader();
                    
                    int adminId;
                    string fullName;
                    
                    if (reader.Read())
                    {
                        adminId = (int)reader["MusteriId"];
                        fullName = reader["Ad"] + " " + reader["Soyad"];
                        reader.Close();
                        
                        // Update password and ensure IsAdmin is true
                        SqlCommand updateCmd = new SqlCommand("UPDATE Musteriler SET Sifre = @Sifre, IsAdmin = 1 WHERE Email = @Email", conn);
                        updateCmd.Parameters.AddWithValue("@Sifre", hashPassword(ADMIN_PASSWORD));
                        updateCmd.Parameters.AddWithValue("@Email", ADMIN_EMAIL);
                        updateCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        reader.Close();
                        
                        // Create admin account
                        SqlCommand insertCmd = new SqlCommand(@"
                            INSERT INTO Musteriler (Ad, Soyad, Email, Sifre, Telefon, Adres, KayitTarihi, IsAdmin) 
                            OUTPUT INSERTED.MusteriId
                            VALUES ('Admin', 'User', @Email, @Sifre, '5551234567', 'Admin Address', GETDATE(), 1)", conn);
                        insertCmd.Parameters.AddWithValue("@Email", ADMIN_EMAIL);
                        insertCmd.Parameters.AddWithValue("@Sifre", hashPassword(ADMIN_PASSWORD));
                        adminId = (int)insertCmd.ExecuteScalar();
                        fullName = "Admin User";
                    }
                    
                    // Set session - LOGIN
                    Session["MusteriId"] = adminId;
                    Session["MusteriAd"] = fullName;
                    Session["IsAdmin"] = true;
                    
                    // Redirect to Admin panel
                    Response.Redirect("/Admin");
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Veritabani hatasi: " + ex.Message;
            }
        }
        else
        {
            errorMessage = "Gecersiz email veya sifre! Bu sayfa sadece yetkili yoneticiler icindir.";
        }
    }
%>
        <% if (!string.IsNullOrEmpty(errorMessage)) { %>
        <div class="alert alert-danger">
            <i class="fas fa-exclamation-circle me-2"></i>
            <%= errorMessage %>
        </div>
        <% } %>
        
        <form method="post">
            <div class="mb-4">
                <label class="form-label fw-bold">
                    <i class="fas fa-envelope me-2 text-muted"></i>Email
                </label>
                <div class="input-group">
                    <span class="input-group-text"><i class="fas fa-at"></i></span>
                    <input type="email" name="email" class="form-control" placeholder="admin@trendyol.com" required>
                </div>
            </div>
            
            <div class="mb-4">
                <label class="form-label fw-bold">
                    <i class="fas fa-lock me-2 text-muted"></i>Sifre
                </label>
                <div class="input-group">
                    <span class="input-group-text"><i class="fas fa-key"></i></span>
                    <input type="password" name="password" class="form-control" placeholder="••••••••" required>
                </div>
            </div>
            
            <div class="d-grid mb-3">
                <button type="submit" class="btn btn-admin btn-primary btn-lg">
                    <i class="fas fa-sign-in-alt me-2"></i>Giris Yap
                </button>
            </div>
        </form>
        
        <div class="text-center">
            <a href="/" class="text-muted text-decoration-none">
                <i class="fas fa-arrow-left me-1"></i>Ana Sayfaya Don
            </a>
        </div>
    </div>
</div>
</body>
</html>