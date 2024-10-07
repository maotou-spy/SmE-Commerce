using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmE_CommerceModels.Models;

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

    public virtual DbSet<ChangeLog> ChangeLogs { get; set; }

    public virtual DbSet<Content> Contents { get; set; }

    public virtual DbSet<ContentCategoryMap> ContentCategoryMaps { get; set; }

    public virtual DbSet<ContentImage> ContentImages { get; set; }

    public virtual DbSet<ContentProduct> ContentProducts { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountCode> DiscountCodes { get; set; }

    public virtual DbSet<DiscountProduct> DiscountProducts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

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
        optionsBuilder.UseNpgsql(GetConnectionString())
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }

    private static string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new InvalidOperationException($"Connection string  not found or empty in appsettings.json");
        return connectionString;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("Addresses_pkey");

            entity.Property(e => e.AddressId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("addressId");
            entity.Property(e => e.Address1)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
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
                .HasConstraintName("fk_useraddress");
        });

        modelBuilder.Entity<BankInfo>(entity =>
        {
            entity.HasKey(e => e.BankInfoId).HasName("BankInfo_pkey");

            entity.ToTable("BankInfo");

            entity.Property(e => e.BankInfoId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("bankInfoId");
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
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
        });

        modelBuilder.Entity<BlogCategory>(entity =>
        {
            entity.HasKey(e => e.BlogCategoryId).HasName("BlogCategories_pkey");

            entity.Property(e => e.BlogCategoryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("blogCategoryId");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
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
            entity.HasKey(e => e.CartItemId).HasName("CartItems_pkey");

            entity.Property(e => e.CartItemId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("cartItemId");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_cartitemproduct");

            entity.HasOne(d => d.User).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_cartitemuser");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("Categories_pkey");

            entity.Property(e => e.CategoryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("categoryId");
            entity.Property(e => e.CategoryImage)
                .HasColumnType("character varying")
                .HasColumnName("categoryImage");
            entity.Property(e => e.CategoryImageHash)
                .HasColumnType("character varying")
                .HasColumnName("categoryImageHash");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
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

        modelBuilder.Entity<ChangeLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("ChangeLog_pkey");

            entity.ToTable("ChangeLog");

            entity.Property(e => e.LogId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("logId");
            entity.Property(e => e.Action)
                .HasMaxLength(10)
                .HasComment("Values: insert, update, delete")
                .HasColumnName("action");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changedAt");
            entity.Property(e => e.ChangedById).HasColumnName("changedById");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Reason)
                .HasMaxLength(255)
                .HasColumnName("reason");
            entity.Property(e => e.RecordId)
                .HasComment("The Id of record has been updated")
                .HasColumnName("recordId");
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .HasColumnName("tableName");

            entity.HasOne(d => d.ChangedBy).WithMany(p => p.ChangeLogs)
                .HasForeignKey(d => d.ChangedById)
                .HasConstraintName("fk_changeloguser");
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("Contents_pkey");

            entity.Property(e => e.ContentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("contentId");
            entity.Property(e => e.AuthorId).HasColumnName("authorId");
            entity.Property(e => e.Content1).HasColumnName("content");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.ExternalId)
                .HasMaxLength(255)
                .HasComment("blogId for blogs, facebookPostId for Facebook posts")
                .HasColumnName("externalId");
            entity.Property(e => e.ExternalType)
                .HasMaxLength(50)
                .HasComment("Values: blog, facebook")
                .HasColumnName("externalType");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.PublishedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("publishedAt");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: draft, pending, published, unpublished, deleted")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
        });

        modelBuilder.Entity<ContentCategoryMap>(entity =>
        {
            entity.HasKey(e => e.ContentCategoryMapId).HasName("ContentCategoryMap_pkey");

            entity.ToTable("ContentCategoryMap");

            entity.Property(e => e.ContentCategoryMapId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("contentCategoryMapId");
            entity.Property(e => e.BlogCategoryId).HasColumnName("blogCategoryId");
            entity.Property(e => e.ContentId).HasColumnName("contentId");

            entity.HasOne(d => d.BlogCategory).WithMany(p => p.ContentCategoryMaps)
                .HasForeignKey(d => d.BlogCategoryId)
                .HasConstraintName("fk_blogcategory");

            entity.HasOne(d => d.Content).WithMany(p => p.ContentCategoryMaps)
                .HasForeignKey(d => d.ContentId)
                .HasConstraintName("fk_contentcategory");
        });

        modelBuilder.Entity<ContentImage>(entity =>
        {
            entity.HasKey(e => e.ContentImageId).HasName("ContentImages_pkey");

            entity.Property(e => e.ContentImageId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("contentImageId");
            entity.Property(e => e.AltText)
                .HasMaxLength(255)
                .HasColumnName("altText");
            entity.Property(e => e.ContentId).HasColumnName("contentId");
            entity.Property(e => e.ImageHash)
                .HasColumnType("character varying")
                .HasColumnName("imageHash");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("imageUrl");

            entity.HasOne(d => d.Content).WithMany(p => p.ContentImages)
                .HasForeignKey(d => d.ContentId)
                .HasConstraintName("fk_contentimagescontent");
        });

        modelBuilder.Entity<ContentProduct>(entity =>
        {
            entity.HasKey(e => e.ContentProductId).HasName("ContentProducts_pkey");

            entity.Property(e => e.ContentProductId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("contentProductId");
            entity.Property(e => e.ContentId).HasColumnName("contentId");
            entity.Property(e => e.ProductId).HasColumnName("productId");

            entity.HasOne(d => d.Content).WithMany(p => p.ContentProducts)
                .HasForeignKey(d => d.ContentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contentproduct");

            entity.HasOne(d => d.Product).WithMany(p => p.ContentProducts)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contentproductproduct");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("Discounts_pkey");

            entity.Property(e => e.DiscountId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("discountId");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
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
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fromDate");
            entity.Property(e => e.IsPercentage).HasColumnName("isPercentage");
            entity.Property(e => e.MaximumDiscount)
                .HasPrecision(15)
                .HasColumnName("maximumDiscount");
            entity.Property(e => e.MinimumOrderAmount)
                .HasPrecision(15)
                .HasColumnName("minimumOrderAmount");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
            entity.Property(e => e.ToDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("toDate");
        });

        modelBuilder.Entity<DiscountCode>(entity =>
        {
            entity.HasKey(e => e.CodeId).HasName("DiscountCodes_pkey");

            entity.Property(e => e.CodeId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("codeId");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DiscountId).HasColumnName("discountId");
            entity.Property(e => e.FromDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("fromDate");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, used, deleted")
                .HasColumnName("status");
            entity.Property(e => e.ToDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("toDate");
            entity.Property(e => e.UserId)
                .HasComment("this code only for this user")
                .HasColumnName("userId");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountCodes)
                .HasForeignKey(d => d.DiscountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_discountcode");

            entity.HasOne(d => d.User).WithMany(p => p.DiscountCodes)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_userdiscountcode");
        });

        modelBuilder.Entity<DiscountProduct>(entity =>
        {
            entity.HasKey(e => e.DiscountProductId).HasName("DiscountProduct_pkey");

            entity.ToTable("DiscountProduct");

            entity.Property(e => e.DiscountProductId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("discountProductId");
            entity.Property(e => e.DiscountId).HasColumnName("discountId");
            entity.Property(e => e.ProductId).HasColumnName("productId");

            entity.HasOne(d => d.Discount).WithMany(p => p.DiscountProducts)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("fk_discount");

            entity.HasOne(d => d.Product).WithMany(p => p.DiscountProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_productdiscount");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("Orders_pkey");

            entity.Property(e => e.OrderId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("orderId");
            entity.Property(e => e.AddressId).HasColumnName("addressId");
            entity.Property(e => e.BOlid)
                .HasColumnType("character varying")
                .HasColumnName("bOLId");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DiscountCodeId).HasColumnName("discountCodeId");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.PointsEarned).HasColumnName("pointsEarned");
            entity.Property(e => e.Reason)
                .HasMaxLength(200)
                .HasColumnName("reason");
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
                .HasConstraintName("fk_addressorder");

            entity.HasOne(d => d.DiscountCode).WithMany(p => p.Orders)
                .HasForeignKey(d => d.DiscountCodeId)
                .HasConstraintName("fk_discountcodeorder");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_userorder");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("OrderItems_pkey");

            entity.Property(e => e.OrderItemId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("orderItemId");
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
                .HasConstraintName("fk_orderitemorder");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_orderitemproduct");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("Payments_pkey");

            entity.Property(e => e.PaymentId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("paymentId");
            entity.Property(e => e.Amount)
                .HasPrecision(15)
                .HasColumnName("amount");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.PaymentMethodId).HasColumnName("paymentMethodId");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: pending, paid, completed")
                .HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_paymentorder");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("fk_paymentmethod");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PaymentMethods_pkey");

            entity.Property(e => e.PaymentMethodId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("paymentMethodId");
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
            entity.HasKey(e => e.ProductId).HasName("Products_pkey");

            entity.Property(e => e.ProductId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("productId");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
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
                .HasDefaultValue(0)
                .HasColumnName("soldQuantity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
            entity.Property(e => e.StockQuantity)
                .HasDefaultValue(0)
                .HasColumnName("stockQuantity");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId).HasName("ProductCategories_pkey");

            entity.Property(e => e.ProductCategoryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("productCategoryId");
            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.ProductId).HasColumnName("productId");

            entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("fk_category");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductCategories)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_product");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("ProductImages_pkey");

            entity.Property(e => e.ImageId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("imageId");
            entity.Property(e => e.AltText)
                .HasMaxLength(255)
                .HasColumnName("altText");
            entity.Property(e => e.ImageHash)
                .HasColumnType("character varying")
                .HasColumnName("imageHash");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_productimage");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("Reviews_pkey");

            entity.Property(e => e.ReviewId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("reviewId");
            entity.Property(e => e.Comment).HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.IsTop).HasColumnName("isTop");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_reviewproduct");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_reviewuser");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("Settings_pkey");

            entity.HasIndex(e => e.Key, "Settings_key_key").IsUnique();

            entity.Property(e => e.SettingId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("settingId");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Key)
                .HasMaxLength(50)
                .HasComment(
                    "Values: shopName, address, phone, email, maximumTopReview, privacyPolicy, termsOfService, pointsConversionRate")
                .HasColumnName("key");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .HasColumnName("value");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("Users_pkey");

            entity.HasIndex(e => e.Email, "Users_email_key").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("userId");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("fullName");
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("lastLogin");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
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
