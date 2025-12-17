<%@ Page Language="C#" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Security.Cryptography" %>
<!DOCTYPE html>
<html>
<head>
    <title>Restoran Admin Olustur</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css">
</head>
<body class="bg-light">
<script runat="server">
    string message = "";
    string messageClass = "";
    System.Collections.Generic.List<string> createdAdmins = new System.Collections.Generic.List<string>();
    System.Data.DataTable restoranlar;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadRestoranlar();
        
        // Otomatik olarak tüm restoranlar için admin oluştur
        if (Request.QueryString["auto"] == "1")
        {
            CreateAllRestaurantAdmins();
        }
        else if (IsPostBack)
        {
            CreateRestaurantAdmin();
        }
    }
    
    void LoadRestoranlar()
    {
        try
        {
            string connectionString = "Data Source=localhost;Initial Catalog=TrendYolYemekDB;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT RestoranId, Ad FROM Restoranlar ORDER BY Ad", conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                restoranlar = new System.Data.DataTable();
                adapter.Fill(restoranlar);
            }
        }
        catch (Exception ex)
        {
            message = "Restoranlar yuklenemedi: " + ex.Message;
            messageClass = "alert-danger";
        }
    }
    
    void CreateAllRestaurantAdmins()
    {
        string connectionString = "Data Source=localhost;Initial Catalog=TrendYolYemekDB;Integrated Security=True;";
        string defaultPassword = "admin123";
        
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                // Her restoran için admin oluştur
                foreach (System.Data.DataRow row in restoranlar.Rows)
                {
                    int restoranId = Convert.ToInt32(row["RestoranId"]);
                    string restoranAd = row["Ad"].ToString().ToLower().Replace(" ", "").Replace("ı", "i").Replace("ö", "o").Replace("ü", "u").Replace("ş", "s").Replace("ç", "c").Replace("ğ", "g");
                    string email = restoranAd + "@admin.com";
                    
                    // Check if admin already exists for this restaurant
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Musteriler WHERE RestoranId = @RestoranId AND IsRestoranAdmin = 1", conn);
                    checkCmd.Parameters.AddWithValue("@RestoranId", restoranId);
                    int count = (int)checkCmd.ExecuteScalar();
                    
                    if (count == 0)
                    {
                        // Create new restaurant admin
                        string hashedPassword = HashPassword(defaultPassword);
                        SqlCommand insertCmd = new SqlCommand(
                            @"INSERT INTO Musteriler (Ad, Soyad, Email, Sifre, KayitTarihi, IsAdmin, IsRestoranAdmin, RestoranId) 
                              VALUES (@Ad, @Soyad, @Email, @Sifre, GETDATE(), 0, 1, @RestoranId)", conn);
                        insertCmd.Parameters.AddWithValue("@Ad", row["Ad"].ToString());
                        insertCmd.Parameters.AddWithValue("@Soyad", "Yönetici");
                        insertCmd.Parameters.AddWithValue("@Email", email);
                        insertCmd.Parameters.AddWithValue("@Sifre", hashedPassword);
                        insertCmd.Parameters.AddWithValue("@RestoranId", restoranId);
                        insertCmd.ExecuteNonQuery();
                        
                        createdAdmins.Add(row["Ad"].ToString() + " → " + email + " / " + defaultPassword);
                    }
                }
                
                if (createdAdmins.Count > 0)
                {
                    message = createdAdmins.Count + " restoran yöneticisi oluşturuldu!";
                    messageClass = "alert-success";
                }
                else
                {
                    message = "Tüm restoranların zaten yöneticisi var.";
                    messageClass = "alert-info";
                }
            }
        }
        catch (Exception ex)
        {
            message = "Hata: " + ex.Message;
            messageClass = "alert-danger";
        }
    }
    
    void CreateRestaurantAdmin()
    {
        string email = Request.Form["email"];
        string password = Request.Form["password"];
        string ad = Request.Form["ad"];
        string soyad = Request.Form["soyad"];
        string restoranIdStr = Request.Form["restoranId"];
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(restoranIdStr))
        {
            message = "Tum alanlar zorunludur!";
            messageClass = "alert-danger";
            return;
        }
        
        int restoranId = int.Parse(restoranIdStr);
        
        try
        {
            string connectionString = "Data Source=localhost;Initial Catalog=TrendYolYemekDB;Integrated Security=True;";
            
            // Hash password
            string hashedPassword = HashPassword(password);
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                
                // Check if email exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Musteriler WHERE Email = @Email", conn);
                checkCmd.Parameters.AddWithValue("@Email", email);
                int count = (int)checkCmd.ExecuteScalar();
                
                if (count > 0)
                {
                    // Update existing user to be restaurant admin
                    SqlCommand updateCmd = new SqlCommand(
                        "UPDATE Musteriler SET Sifre = @Sifre, IsRestoranAdmin = 1, RestoranId = @RestoranId WHERE Email = @Email", conn);
                    updateCmd.Parameters.AddWithValue("@Sifre", hashedPassword);
                    updateCmd.Parameters.AddWithValue("@RestoranId", restoranId);
                    updateCmd.Parameters.AddWithValue("@Email", email);
                    updateCmd.ExecuteNonQuery();
                    
                    message = "Kullanici restoran yoneticisi olarak guncellendi! Email: " + email + ", Sifre: " + password;
                    messageClass = "alert-success";
                }
                else
                {
                    // Create new restaurant admin
                    SqlCommand insertCmd = new SqlCommand(
                        @"INSERT INTO Musteriler (Ad, Soyad, Email, Sifre, KayitTarihi, IsAdmin, IsRestoranAdmin, RestoranId) 
                          VALUES (@Ad, @Soyad, @Email, @Sifre, GETDATE(), 0, 1, @RestoranId)", conn);
                    insertCmd.Parameters.AddWithValue("@Ad", ad ?? "Restoran");
                    insertCmd.Parameters.AddWithValue("@Soyad", soyad ?? "Yoneticisi");
                    insertCmd.Parameters.AddWithValue("@Email", email);
                    insertCmd.Parameters.AddWithValue("@Sifre", hashedPassword);
                    insertCmd.Parameters.AddWithValue("@RestoranId", restoranId);
                    insertCmd.ExecuteNonQuery();
                    
                    message = "Restoran yoneticisi olusturuldu! Email: " + email + ", Sifre: " + password;
                    messageClass = "alert-success";
                }
            }
        }
        catch (Exception ex)
        {
            message = "Hata: " + ex.Message;
            messageClass = "alert-danger";
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
    <div class="row justify-content-center">
        <div class="col-lg-8">
            <!-- Auto Create All Admins Button -->
            <div class="card shadow border-0 mb-4 bg-primary text-white">
                <div class="card-body text-center py-4">
                    <h4><i class="fas fa-magic me-2"></i>Otomatik Tum Restoran Yoneticilerini Olustur</h4>
                    <p class="mb-3">Tek tikla her restoran icin yonetici olustur (Sifre: admin123)</p>
                    <a href="?auto=1" class="btn btn-light btn-lg">
                        <i class="fas fa-bolt me-2"></i>Tum Yoneticileri Olustur
                    </a>
                </div>
            </div>
            
            <% if (createdAdmins.Count > 0) { %>
                <div class="card shadow border-0 mb-4">
                    <div class="card-header bg-success text-white">
                        <h5 class="mb-0"><i class="fas fa-check-circle me-2"></i>Olusturulan Yoneticiler</h5>
                    </div>
                    <div class="card-body">
                        <ul class="list-group list-group-flush">
                            <% foreach (string admin in createdAdmins) { %>
                                <li class="list-group-item"><i class="fas fa-user-tie text-success me-2"></i><%= admin %></li>
                            <% } %>
                        </ul>
                    </div>
                </div>
            <% } %>
            
            <div class="card shadow-lg border-0">
                <div class="card-header bg-success text-white py-4 text-center">
                    <h3 class="mb-0">
                        <i class="fas fa-user-tie me-2"></i>Restoran Yoneticisi Olustur
                    </h3>
                </div>
                <div class="card-body p-5">
                    <% if (!string.IsNullOrEmpty(message)) { %>
                        <div class="alert <%= messageClass %> mb-4">
                            <i class="fas fa-info-circle me-2"></i><%= message %>
                        </div>
                    <% } %>
                    
                    <form method="post">
                        <div class="row mb-3">
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Ad</label>
                                <input type="text" name="ad" class="form-control form-control-lg" value="Restoran" required>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Soyad</label>
                                <input type="text" name="soyad" class="form-control form-control-lg" value="Yoneticisi" required>
                            </div>
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label fw-bold">Email</label>
                            <input type="email" name="email" class="form-control form-control-lg" value="restoran1@trendyol.com" required>
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label fw-bold">Sifre</label>
                            <input type="text" name="password" class="form-control form-control-lg" value="admin123" required>
                            <small class="text-muted">Sifre gosteriliyor - guvenligi icin degistirin</small>
                        </div>
                        
                        <div class="mb-4">
                            <label class="form-label fw-bold">Restoran</label>
                            <select name="restoranId" class="form-select form-select-lg" required>
                                <% if (restoranlar != null) { foreach (System.Data.DataRow row in restoranlar.Rows) { %>
                                    <option value="<%= row["RestoranId"] %>"><%= row["Ad"] %></option>
                                <% } } %>
                            </select>
                        </div>
                        
                        <button type="submit" class="btn btn-success btn-lg w-100">
                            <i class="fas fa-user-plus me-2"></i>Restoran Yoneticisi Olustur
                        </button>
                    </form>
                    
                    <hr class="my-4">
                    
                    <div class="text-center">
                        <a href="/Account/Login" class="btn btn-outline-primary">
                            <i class="fas fa-sign-in-alt me-2"></i>Giris Yap
                        </a>
                        <a href="/RestaurantAdmin/Dashboard" class="btn btn-outline-success ms-2">
                            <i class="fas fa-tachometer-alt me-2"></i>Yonetim Paneli
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</body>
</html>
