using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmE_CommerceModels.Models;

namespace SmE_CommerceModels.DBContext;

public partial class SmECommerceContext : DbContext
{
    public SmECommerceContext()
    {
    }

    public SmECommerceContext(DbContextOptions<SmECommerceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<BankInfo> BankInfos { get; set; }

    public virtual DbSet<BlogCategory> BlogCategories { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Content> Contents { get; set; }

    public virtual DbSet<ContentCategoryMap> ContentCategoryMaps { get; set; }

    public virtual DbSet<ContentImage> ContentImages { get; set; }

    public virtual DbSet<ContentProduct> ContentProducts { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<DiscountCode> DiscountCodes { get; set; }

    public virtual DbSet<DiscountProduct> DiscountProducts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductVariant> ProductVariants { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VariantAttribute> VariantAttributes { get; set; }

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
            entity.Property(e => e.IsDefault)
                .HasDefaultValue(false)
                .HasColumnName("isDefault");
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

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("AuditLogs_pkey");

            entity.Property(e => e.LogId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("logId");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(50)
                .HasColumnName("ipAddress");
            entity.Property(e => e.NewValue)
                .HasColumnType("jsonb")
                .HasColumnName("newValue");
            entity.Property(e => e.OldValue)
                .HasColumnType("jsonb")
                .HasColumnName("oldValue");
            entity.Property(e => e.RecordId).HasColumnName("recordId");
            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .HasColumnName("tableName");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(255)
                .HasColumnName("userAgent");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("AuditLogs_userId_fkey");
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

            entity.HasIndex(e => e.Slug, "Categories_slug_key").IsUnique();

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
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0)
                .HasColumnName("displayOrder");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("Contents_pkey");

            entity.HasIndex(e => e.Slug, "Contents_slug_key").IsUnique();

            entity.HasIndex(e => e.Status, "idx_contents_status");

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
            entity.Property(e => e.Keywords)
                .HasColumnType("character varying[]")
                .HasColumnName("keywords");
            entity.Property(e => e.MetaDescription).HasColumnName("metaDescription");
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(255)
                .HasColumnName("metaTitle");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.PublishedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("publishedAt");
            entity.Property(e => e.ShortDescription).HasColumnName("shortDescription");
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: draft, pending, published, unpublished, deleted")
                .HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.ViewCount)
                .HasDefaultValue(0)
                .HasColumnName("viewCount");
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
            entity.Property(e => e.IsFirstOrder)
                .HasDefaultValue(false)
                .HasColumnName("isFirstOrder");
            entity.Property(e => e.IsPercentage).HasColumnName("isPercentage");
            entity.Property(e => e.MaxQuantity).HasColumnName("maxQuantity");
            entity.Property(e => e.MaximumDiscount)
                .HasPrecision(15)
                .HasColumnName("maximumDiscount");
            entity.Property(e => e.MinQuantity).HasColumnName("minQuantity");
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
            entity.Property(e => e.UsageLimit).HasColumnName("usageLimit");
            entity.Property(e => e.UsedCount)
                .HasDefaultValue(0)
                .HasColumnName("usedCount");
        });

        modelBuilder.Entity<DiscountCode>(entity =>
        {
            entity.HasKey(e => e.CodeId).HasName("DiscountCodes_pkey");

            entity.HasIndex(e => e.Status, "idx_discountcodes_status");

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

            entity.HasIndex(e => e.OrderCode, "Orders_orderCode_key").IsUnique();

            entity.HasIndex(e => e.Status, "idx_orders_status");

            entity.HasIndex(e => e.UserId, "idx_orders_userid");

            entity.Property(e => e.OrderId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("orderId");
            entity.Property(e => e.ActualDeliveryDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("actualDeliveryDate");
            entity.Property(e => e.AddressId).HasColumnName("addressId");
            entity.Property(e => e.CancelReason)
                .HasMaxLength(200)
                .HasColumnName("cancelReason");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DiscountCodeId).HasColumnName("discountCodeId");
            entity.Property(e => e.Discountamount)
                .HasPrecision(15)
                .HasColumnName("discountamount");
            entity.Property(e => e.EstimatedDeliveryDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("estimatedDeliveryDate");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Note)
                .HasColumnType("character varying")
                .HasColumnName("note");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(50)
                .HasColumnName("orderCode");
            entity.Property(e => e.PointsEarned).HasColumnName("pointsEarned");
            entity.Property(e => e.ReturnReason)
                .HasMaxLength(200)
                .HasColumnName("returnReason");
            entity.Property(e => e.ShippingFee)
                .HasPrecision(15)
                .HasColumnName("shippingFee");
            entity.Property(e => e.ShippingCode)
                .HasColumnType("character varying")
                .HasColumnName("shippingCode");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: pending, processing, completed, cancelled, rejected, returned")
                .HasColumnName("status");
            entity.Property(e => e.SubTotal)
                .HasPrecision(15)
                .HasColumnName("subTotal");
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
            entity.Property(e => e.ProductName)
                .HasMaxLength(100)
                .HasColumnName("productName");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, deleted")
                .HasColumnName("status");
            entity.Property(e => e.VariantId).HasColumnName("variantId");
            entity.Property(e => e.VariantName)
                .HasMaxLength(100)
                .HasColumnName("variantName");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("fk_orderitemorder");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("fk_orderitemproduct");

            entity.HasOne(d => d.Variant).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.VariantId)
                .HasConstraintName("OrderItems_variantId_fkey");
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("OrderStatusHistory_pkey");

            entity.ToTable("OrderStatusHistory");

            entity.Property(e => e.HistoryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("historyId");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changedAt");
            entity.Property(e => e.ChangedById).HasColumnName("changedById");
            entity.Property(e => e.FromStatus)
                .HasMaxLength(50)
                .HasColumnName("fromStatus");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.OrderId).HasColumnName("orderId");
            entity.Property(e => e.ToStatus)
                .HasMaxLength(50)
                .HasColumnName("toStatus");

            entity.HasOne(d => d.ChangedBy).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.ChangedById)
                .HasConstraintName("OrderStatusHistory_changedById_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderStatusHistories)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderStatusHistory_orderId_fkey");
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

            entity.HasIndex(e => e.Slug, "Products_slug_key").IsUnique();

            entity.HasIndex(e => e.Status, "idx_products_status");

            entity.Property(e => e.ProductId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("productId");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsTopSeller)
                .HasDefaultValue(false)
                .HasColumnName("isTopSeller");
            entity.Property(e => e.Keywords)
                .HasColumnType("character varying[]")
                .HasColumnName("keywords");
            entity.Property(e => e.MetaDescription).HasColumnName("metaDescription");
            entity.Property(e => e.MetaTitle)
                .HasMaxLength(255)
                .HasColumnName("metaTitle");
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
            entity.Property(e => e.Slug)
                .HasMaxLength(255)
                .HasColumnName("slug");
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

        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            entity.HasKey(e => e.Attributeid).HasName("productattributes_pk");

            entity.Property(e => e.Attributeid)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("attributeid");
            entity.Property(e => e.Attributename)
                .HasMaxLength(100)
                .HasColumnName("attributename");
            entity.Property(e => e.Attributevalue)
                .HasMaxLength(255)
                .HasColumnName("attributevalue");
            entity.Property(e => e.Productid).HasColumnName("productid");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductAttributes)
                .HasForeignKey(d => d.Productid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_productproductattribute");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId).HasName("ProductCategories_pkey");

            entity.Property(e => e.ProductCategoryId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("productCategoryId");
            entity.Property(e => e.CategoryId).HasColumnName("categoryId");
            entity.Property(e => e.DisplayOrder)
                .HasDefaultValue(0)
                .HasColumnName("displayOrder");
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

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.VariantId).HasName("productvariants_pk");

            entity.Property(e => e.VariantId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("variantId");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.ModifiedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("modifiedAt");
            entity.Property(e => e.ModifiedById).HasColumnName("modifiedById");
            entity.Property(e => e.Price)
                .HasPrecision(15)
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.Sku)
                .HasMaxLength(50)
                .HasColumnName("sku");
            entity.Property(e => e.SoldQuantity)
                .HasDefaultValue(0)
                .HasColumnName("soldQuantity");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.StockQuantity)
                .HasDefaultValue(0)
                .HasColumnName("stockQuantity");
            entity.Property(e => e.VariantName)
                .HasMaxLength(100)
                .HasColumnName("variantName");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("productvariants_products_productid_fk");
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
                .HasComment("Values: shopName, address, phone, email, maximumTopReview, privacyPolicy, termsOfService, pointsConversionRate")
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

            entity.HasIndex(e => e.Username, "Users_username_key").IsUnique();

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("userId");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.CreateById).HasColumnName("createById");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("fullName");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.IsEmailVerified)
                .HasDefaultValue(false)
                .HasColumnName("isEmailVerified");
            entity.Property(e => e.IsPhoneVerified)
                .HasDefaultValue(false)
                .HasColumnName("isPhoneVerified");
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
            entity.Property(e => e.ResetPasswordExpires)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("resetPasswordExpires");
            entity.Property(e => e.ResetPasswordToken)
                .HasMaxLength(255)
                .HasColumnName("resetPasswordToken");
            entity.Property(e => e.Role)
                .HasMaxLength(30)
                .HasColumnName("role");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasComment("Values: active, inactive, suspended")
                .HasColumnName("status");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<VariantAttribute>(entity =>
        {
            entity.HasKey(e => e.AttributeId).HasName("variantattributes_pk");

            entity.Property(e => e.AttributeId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("attributeId");
            entity.Property(e => e.AttributeName)
                .HasMaxLength(100)
                .HasColumnName("attributeName");
            entity.Property(e => e.AttributeValue)
                .HasMaxLength(255)
                .HasColumnName("attributeValue");
            entity.Property(e => e.VariantId).HasColumnName("variantId");

            entity.HasOne(d => d.Variant).WithMany(p => p.VariantAttributes)
                .HasForeignKey(d => d.VariantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("variantattributes_productvariants_variantid_fk");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
