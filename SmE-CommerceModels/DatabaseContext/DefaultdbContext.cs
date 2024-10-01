using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmE_CommerceModels.Objects;

namespace SmE_CommerceModels.DatabaseContext;

public partial class DefaultdbContext : DbContext
{
    public DefaultdbContext()
    {
    }

    public DefaultdbContext(DbContextOptions<DefaultdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<BankInfo> BankInfos { get; set; }

    public virtual DbSet<BlogCategory> BlogCategories { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Content> Contents { get; set; }

    public virtual DbSet<ContentCategoryMap> ContentCategoryMaps { get; set; }

    public virtual DbSet<ContentImage> ContentImages { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountCode> DiscountCodes { get; set; }

    public virtual DbSet<DiscountProduct> DiscountProducts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderPoint> OrderPoints { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = GetConnectionString("DefaultConnection");
        var serverVersion = GetConnectionString("ServerVersion");
        optionsBuilder.UseMySql(connectionString, ServerVersion.Parse(serverVersion));
    }

    private static string GetConnectionString(string key)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        return config.GetConnectionString(key) ?? string.Empty;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PRIMARY");

            entity.HasIndex(e => e.UserId, "userId");

            entity.Property(e => e.AddressId).HasColumnName("addressId");
            entity.Property(e => e.Address1)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.ReceiverName)
                .HasMaxLength(100)
                .HasColumnName("receiverName");
            entity.Property(e => e.ReceiverPhone)
                .HasMaxLength(12)
                .HasColumnName("receiverPhone");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Ward)
                .HasMaxLength(100)
                .HasColumnName("ward");

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("Addresses_ibfk_1");
        });

        modelBuilder.Entity<BankInfo>(entity =>
        {
            entity.HasKey(e => e.BankInfoId).HasName("PRIMARY");

            entity.ToTable("BankInfo");

            entity.Property(e => e.BankInfoId).HasColumnName("bankInfoId");
            entity.Property(e => e.AccountHolderName)
                .HasMaxLength(100)
                .HasColumnName("accountHolderName");
            entity.Property(e => e.AccountNumber)
                .HasMaxLength(50)
                .HasColumnName("accountNumber");
            entity.Property(e => e.BankCode)
                .HasMaxLength(10)
                .HasColumnName("bankCode");
            entity.Property(e => e.BankLogoUrl)
                .HasMaxLength(100)
                .HasColumnName("bankLogoUrl");
            entity.Property(e => e.BankName)
                .HasMaxLength(100)
                .HasColumnName("bankName");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
        });

        modelBuilder.Entity<BlogCategory>(entity =>
        {
            entity.HasKey(e => e.BlogCategoryId).HasName("PRIMARY");

            entity.Property(e => e.BlogCategoryId).HasColumnName("blogCategoryId");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PRIMARY");

            entity.HasIndex(e => e.ProductId, "productId");

            entity.HasIndex(e => e.UserId, "userId");

            entity.Property(e => e.CartItemId).HasColumnName("cartItemId");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("CartItems_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("CartItems_ibfk_1");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.CategoryImage)
                .HasMaxLength(255)
                .HasColumnName("categoryImage");
            entity.Property(e => e.CategoryImageHash)
                .HasMaxLength(255)
                .HasColumnName("categoryImageHash");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("PRIMARY");

            entity.HasIndex(e => e.ProductId, "Contents_ibfk_2_idx");

            entity.HasIndex(e => e.AuthorId, "authorId");

            entity.Property(e => e.ContentId).HasColumnName("contentId");
            entity.Property(e => e.AuthorId).HasColumnName("authorId");
            entity.Property(e => e.Content1).HasColumnName("content");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.ExternalId)
                .HasMaxLength(255)
                .HasComment("blogId for blogs, facebookPostId for Facebook posts")
                .HasColumnName("externalId");
            entity.Property(e => e.ExternalType)
                .HasMaxLength(50)
                .HasComment("Values: blog, facebook")
                .HasColumnName("externalType");
            entity.Property(e => e.IsFeature)
                .HasDefaultValueSql("b'0'")
                .HasComment("Bring to homepage")
                .HasColumnType("bit(1)")
                .HasColumnName("isFeature");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.PublishedAt)
                .HasColumnType("datetime")
                .HasColumnName("publishedAt");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: draft, pending, published, unpublished, deleted")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Author).WithMany(p => p.Contents)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("Contents_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.Contents)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Contents_ibfk_2");
        });

        modelBuilder.Entity<ContentCategoryMap>(entity =>
        {
            entity.HasKey(e => e.ContentCategoryMapId).HasName("PRIMARY");

            entity.ToTable("ContentCategoryMap");

            entity.HasIndex(e => e.BlogCategoryId, "blogCategoryId");

            entity.HasIndex(e => e.ContentId, "contentId");

            entity.Property(e => e.ContentCategoryMapId).HasColumnName("contentCategoryMapId");
            entity.Property(e => e.BlogCategoryId).HasColumnName("blogCategoryId");
            entity.Property(e => e.ContentId).HasColumnName("contentId");

            entity.HasOne(d => d.BlogCategory).WithMany(p => p.ContentCategoryMaps)
                .HasForeignKey(d => d.BlogCategoryId)
                .HasConstraintName("ContentCategoryMap_ibfk_1");

            entity.HasOne(d => d.Content).WithMany(p => p.ContentCategoryMaps)
                .HasForeignKey(d => d.ContentId)
                .HasConstraintName("ContentCategoryMap_ibfk_2");
        });

        modelBuilder.Entity<ContentImage>(entity =>
        {
            entity.HasKey(e => e.ContentImageId).HasName("PRIMARY");

            entity.HasIndex(e => e.ContentId, "contentId");

            entity.Property(e => e.ContentImageId).HasColumnName("contentImageId");
            entity.Property(e => e.AltText)
                .HasMaxLength(255)
                .HasColumnName("altText");
            entity.Property(e => e.ContentId).HasColumnName("contentId");
            entity.Property(e => e.ImageHash)
                .HasMaxLength(255)
                .HasColumnName("imageHash");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("imageUrl");

            entity.HasOne(d => d.Content).WithMany(p => p.ContentImages)
                .HasForeignKey(d => d.ContentId)
                .HasConstraintName("ContentImages_ibfk_1");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PRIMARY");

            entity.Property(e => e.DiscountId).HasColumnName("discountId");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.DiscountName)
                .HasMaxLength(100)
                .HasColumnName("discountName");
            entity.Property(e => e.DiscountValue)
                .HasPrecision(15, 2)
                .HasColumnName("discountValue");
            entity.Property(e => e.FromDate)
                .HasColumnType("datetime")
                .HasColumnName("fromDate");
            entity.Property(e => e.IsPercentage).HasColumnName("isPercentage");
            entity.Property(e => e.MaximumDiscount)
                .HasPrecision(15)
                .HasColumnName("maximumDiscount");
            entity.Property(e => e.MinimumOrderAmount)
                .HasPrecision(15)
                .HasColumnName("minimumOrderAmount");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
            entity.Property(e => e.ToDate)
                .HasColumnType("datetime")
                .HasColumnName("toDate");
        });

        modelBuilder.Entity<DiscountCode>(entity =>
        {
            entity.HasKey(e => e.CodeId).HasName("PRIMARY");

            entity.HasIndex(e => e.DiscountId, "discountId");

            entity.Property(e => e.CodeId).HasColumnName("codeId");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.DiscountId).HasColumnName("discountId");
            entity.Property(e => e.FromDate)
                .HasColumnType("datetime")
                .HasColumnName("fromDate");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, used, deleted")
                .HasColumnName("status");
            entity.Property(e => e.ToDate)
                .HasColumnType("datetime")
                .HasColumnName("toDate");
            entity.Property(e => e.UserId)
                .HasComment("this code only for this user")
                .HasColumnName("userId");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountCodes)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("DiscountCodes_ibfk_1");
        });

        modelBuilder.Entity<DiscountProduct>(entity =>
        {
            entity.HasKey(e => e.DiscountProductId).HasName("PRIMARY");

            entity.ToTable("DiscountProduct");

            entity.HasIndex(e => e.DiscountId, "discountId");

            entity.HasIndex(e => e.ProductId, "productId");

            entity.Property(e => e.DiscountProductId).HasColumnName("discountProductId");
            entity.Property(e => e.DiscountId).HasColumnName("discountId");
            entity.Property(e => e.ProductId).HasColumnName("productId");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountProducts)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("DiscountProduct_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.DiscountProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("DiscountProduct_ibfk_2");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PRIMARY");

            entity.HasIndex(e => e.AddressId, "addressId");

            entity.HasIndex(e => e.DiscountCodeId, "discountCodeId");

            entity.HasIndex(e => e.UserId, "userId");

            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.AddressId).HasColumnName("addressId");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.DiscountCodeId).HasColumnName("discountCodeId");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.PointsEarned).HasColumnName("pointsEarned");
            entity.Property(e => e.Reason)
                .HasMaxLength(200)
                .HasColumnName("reason");
            entity.Property(e => e.ShippingCode)
                .HasMaxLength(255)
                .HasColumnName("shippingCode");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: pending, processing, completed, cancelled, rejected, returned")
                .HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasPrecision(15)
                .HasColumnName("totalAmount");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Address).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("Orders_ibfk_3");

            entity.HasOne(d => d.DiscountCode).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountCodeId)
                .HasConstraintName("Orders_ibfk_4");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Orders_ibfk_1");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PRIMARY");

            entity.HasIndex(e => e.OrderId, "orderId");

            entity.HasIndex(e => e.ProductId, "productId");

            entity.Property(e => e.OrderItemId).HasColumnName("orderItemId");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.Price)
                .HasPrecision(15)
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("OrderItems_ibfk_1");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("OrderItems_ibfk_2");
        });

        modelBuilder.Entity<OrderPoint>(entity =>
        {
            entity.HasKey(e => e.OrderPointId).HasName("PRIMARY");

            entity.HasIndex(e => e.OrderId, "orderId");

            entity.Property(e => e.OrderPointId).HasColumnName("orderPointId");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.PointsEarned).HasColumnName("pointsEarned");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderPoints)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("OrderPoints_ibfk_1");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PRIMARY");

            entity.HasIndex(e => e.OrderId, "orderId");

            entity.HasIndex(e => e.PaymentMethodId, "paymentMethodId");

            entity.Property(e => e.PaymentId).HasColumnName("paymentId");
            entity.Property(e => e.Amount)
                .HasPrecision(15)
                .HasColumnName("amount");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.PaymentMethodId).HasColumnName("paymentMethodId");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: pending, completed")
                .HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("Payments_ibfk_1");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("Payments_ibfk_2");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PRIMARY");

            entity.Property(e => e.PaymentMethodId).HasColumnName("paymentMethodId");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PRIMARY");

            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsFeature)
                .HasComment("feature products")
                .HasColumnType("bit(1)")
                .HasColumnName("isFeature");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasPrecision(15)
                .HasColumnName("price");
            entity.Property(e => e.ProductCode)
                .HasMaxLength(50)
                .HasColumnName("productCode");
            entity.Property(e => e.SoldQuantity)
                .HasDefaultValueSql("'0'")
                .HasColumnName("soldQuantity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
            entity.Property(e => e.StockQuantity)
                .HasDefaultValueSql("'0'")
                .HasColumnName("stockQuantity");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId).HasName("PRIMARY");

            entity.HasIndex(e => e.CategoryId, "categoryId");

            entity.HasIndex(e => e.ProductId, "productId");

            entity.Property(e => e.ProductCategoryId).HasColumnName("productCategoryId");
            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.ProductId).HasColumnName("productId");

            entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("ProductCategories_ibfk_2");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("ProductCategories_ibfk_1");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PRIMARY");

            entity.HasIndex(e => e.ProductId, "productId");

            entity.Property(e => e.ImageId).HasColumnName("imageId");
            entity.Property(e => e.AltText)
                .HasMaxLength(255)
                .HasColumnName("altText");
            entity.Property(e => e.ImageHash)
                .HasMaxLength(255)
                .HasColumnName("imageHash");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("ProductImages_ibfk_1");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PRIMARY");

            entity.HasIndex(e => e.ProductId, "productId");

            entity.Property(e => e.ReviewId).HasColumnName("reviewId");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.IsFeature)
                .HasColumnType("bit(1)")
                .HasColumnName("isFeature");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("Reviews_ibfk_1");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("PRIMARY");

            entity.HasIndex(e => e.Key, "key").IsUnique();

            entity.Property(e => e.SettingId).HasColumnName("settingId");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Key)
                .HasMaxLength(50)
                .HasComment("Values: shopName, address, phone, email, logoUrl, maximumTopReview, privacyPolicy, termsOfService, pointsConversionRate")
                .HasColumnName("key");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .HasColumnName("value");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.CreatedById).HasColumnName("createdById");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("fullName");
            entity.Property(e => e.LastLogin)
                .HasColumnType("datetime")
                .HasColumnName("lastLogin");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("datetime")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("passwordHash");
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .HasColumnName("phone");
            entity.Property(e => e.Point).HasColumnName("point");
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, suspended")
                .HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
