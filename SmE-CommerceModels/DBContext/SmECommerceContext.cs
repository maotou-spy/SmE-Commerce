using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmE_CommerceModels.Models;

namespace SmE_CommerceModels.DBContext;

public partial class SmECommerceContext : DbContext
{
    public SmECommerceContext() { }

    public SmECommerceContext(DbContextOptions<SmECommerceContext> options)
        : base(options) { }

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

    public virtual DbSet<VariantName> VariantNames { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseNpgsql(GetConnectionString())
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
            throw new InvalidOperationException(
                "Connection string  not found or empty in appsettings.json"
            );
        return connectionString;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("Addresses_pkey");

            entity.Property(e => e.AddressId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.IsDefault).HasDefaultValue(false);
            entity.Property(e => e.Status).HasComment("Values: active, inactive, deleted");

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.AddressCreateBies)
                .HasConstraintName("Addresses_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.AddressModifiedBies)
                .HasConstraintName("Addresses_modifiedById_fk");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.AddressUsers)
                .HasConstraintName("fk_useraddress");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("AuditLogs_pkey");

            entity.Property(e => e.LogId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.AuditLogs)
                .HasConstraintName("AuditLogs_userId_fkey");
        });

        modelBuilder.Entity<BankInfo>(entity =>
        {
            entity.HasKey(e => e.BankInfoId).HasName("BankInfo_pkey");

            entity.Property(e => e.BankInfoId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Status).HasComment("Values: active, inactive, deleted");

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.BankInfoCreateBies)
                .HasConstraintName("BankInfo_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.BankInfoModifiedBies)
                .HasConstraintName("BankInfo_modifiedById_fk");
        });

        modelBuilder.Entity<BlogCategory>(entity =>
        {
            entity.HasKey(e => e.BlogCategoryId).HasName("BlogCategories_pkey");

            entity.Property(e => e.BlogCategoryId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Status).HasComment("Values: active, inactive, deleted");

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.BlogCategoryCreateBies)
                .HasConstraintName("BlogCategories_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.BlogCategoryModifiedBies)
                .HasConstraintName("BlogCategories_modifiedById_fk");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("CartItems_pkey");

            entity.Property(e => e.CartItemId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Price).HasComment("Price of the product when added to the cart");

            entity
                .HasOne(d => d.Product)
                .WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_CartItem_Product");

            entity
                .HasOne(d => d.ProductVariant)
                .WithMany(p => p.CartItems)
                .HasConstraintName("fk_cartitemproductvariant");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.CartItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cartitemuser");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("Categories_pkey");

            entity.Property(e => e.CategoryId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Status).HasComment("Values: active, inactive, deleted");

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.CategoryCreateBies)
                .HasConstraintName("Categories_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.CategoryModifiedBies)
                .HasConstraintName("Categories_modifiedById_fk");
        });

        modelBuilder.Entity<Content>(entity =>
        {
            entity.HasKey(e => e.ContentId).HasName("Contents_pkey");

            entity.Property(e => e.ContentId).HasDefaultValueSql("gen_random_uuid()");
            entity
                .Property(e => e.ExternalId)
                .HasComment("blogId for blogs, facebookPostId for Facebook posts");
            entity.Property(e => e.ExternalType).HasComment("Values: blog, facebook");
            entity
                .Property(e => e.Status)
                .HasComment("Values: draft, pending, published, unpublished, deleted");
            entity.Property(e => e.ViewCount).HasDefaultValue(0);

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.ContentCreateBies)
                .HasConstraintName("Contents_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.ContentModifiedBies)
                .HasConstraintName("Contents_modifiedById_fk");
        });

        modelBuilder.Entity<ContentCategoryMap>(entity =>
        {
            entity.HasKey(e => e.ContentCategoryMapId).HasName("ContentCategoryMap_pkey");

            entity.Property(e => e.ContentCategoryMapId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.BlogCategory)
                .WithMany(p => p.ContentCategoryMaps)
                .HasConstraintName("fk_blogcategory");

            entity
                .HasOne(d => d.Content)
                .WithMany(p => p.ContentCategoryMaps)
                .HasConstraintName("fk_contentcategory");
        });

        modelBuilder.Entity<ContentImage>(entity =>
        {
            entity.HasKey(e => e.ContentImageId).HasName("ContentImages_pkey");

            entity.Property(e => e.ContentImageId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.Content)
                .WithMany(p => p.ContentImages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contentimagescontent");
        });

        modelBuilder.Entity<ContentProduct>(entity =>
        {
            entity.HasKey(e => e.ContentProductId).HasName("ContentProducts_pkey");

            entity.Property(e => e.ContentProductId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.Content)
                .WithMany(p => p.ContentProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contentproduct");

            entity
                .HasOne(d => d.Product)
                .WithMany(p => p.ContentProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_contentproductproduct");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("Discounts_pkey");

            entity.Property(e => e.DiscountId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.IsFirstOrder).HasDefaultValue(false);
            entity.Property(e => e.Status).HasComment("Values: active, inactive, deleted");

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.DiscountCreateBies)
                .HasConstraintName("Discounts_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.DiscountModifiedBies)
                .HasConstraintName("Discounts_modifiedById_fk");
        });

        modelBuilder.Entity<DiscountCode>(entity =>
        {
            entity.HasKey(e => e.CodeId).HasName("DiscountCodes_pkey");

            entity.Property(e => e.CodeId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Status).HasComment("Values: active, inactive, used, deleted");
            entity.Property(e => e.UserId).HasComment("this code only for this user");

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.DiscountCodeCreateBies)
                .HasConstraintName("DiscountCodes_createById_fk");

            entity
                .HasOne(d => d.Discount)
                .WithMany(p => p.DiscountCodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_discountcode");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.DiscountCodeModifiedBies)
                .HasConstraintName("DiscountCodes_modifiedById_fk");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.DiscountCodeUsers)
                .HasConstraintName("fk_userdiscountcode");
        });

        modelBuilder.Entity<DiscountProduct>(entity =>
        {
            entity.HasKey(e => e.DiscountProductId).HasName("DiscountProduct_pkey");

            entity.Property(e => e.DiscountProductId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.Discount)
                .WithMany(p => p.DiscountProducts)
                .HasConstraintName("fk_discount");

            entity
                .HasOne(d => d.Product)
                .WithMany(p => p.DiscountProducts)
                .HasConstraintName("fk_productdiscount");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("Orders_pkey");

            entity.Property(e => e.OrderId).HasDefaultValueSql("gen_random_uuid()");
            entity
                .Property(e => e.OrderCode)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValueSql(
                    "concat('TMDS', to_char(NOW(), 'YYMMDD'), '-', lpad(nextval('order_code_seq')::text, 6, '0'))"
                );
            entity
                .Property(e => e.Status)
                .HasComment(
                    "Values: pending, processing, completed, cancelled, rejected, returned"
                );
            entity
                .Property(e => e.OrderCode)
                .HasDefaultValueSql(
                    "concat('TMDS', to_char(now(), 'YYMMDD'::text), '-', lpad((nextval('order_code_seq'::regclass))::text, 6, '0'::text))"
                );
            entity.Property(e => e.PointsUsed).HasDefaultValue(0);
            entity
                .Property(e => e.Status)
                .HasComment(
                    "Values: pending, processing, completed, cancelled, rejected, returned"
                );

            entity
                .HasOne(d => d.Address)
                .WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_addressorder");

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.OrderCreateBies)
                .HasConstraintName("Orders_createById_fk");

            entity
                .HasOne(d => d.DiscountCode)
                .WithMany(p => p.Orders)
                .HasConstraintName("fk_discountcodeorder");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.OrderModifiedBies)
                .HasConstraintName("Orders_modifiedById_fk");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.OrderUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_userorder");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("OrderItems_pkey");

            entity.Property(e => e.OrderItemId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.Order)
                .WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orderitemorder");

            entity
                .HasOne(d => d.Product)
                .WithMany(p => p.OrderItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_orderitemproduct");

            entity
                .HasOne(d => d.ProductVariant)
                .WithMany(p => p.OrderItems)
                .HasConstraintName("OrderItems_variantId_fkey");
        });

        modelBuilder.Entity<OrderStatusHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("OrderStatusHistory_pkey");

            entity.Property(e => e.HistoryId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.ModifiedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.OrderStatusHistories)
                .HasConstraintName("OrderStatusHistory_changedById_fkey");

            entity
                .HasOne(d => d.Order)
                .WithMany(p => p.OrderStatusHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("OrderStatusHistory_orderId_fkey");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("Payments_pkey");

            entity.Property(e => e.PaymentId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Status).HasComment("Values: pending, paid, completed");

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.PaymentCreateBies)
                .HasConstraintName("Payments_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.PaymentModifiedBies)
                .HasConstraintName("Payments_modifiedById_fk");

            entity
                .HasOne(d => d.Order)
                .WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_paymentorder");

            entity
                .HasOne(d => d.PaymentMethod)
                .WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_paymentmethod");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentMethodId).HasName("PaymentMethods_pkey");

            entity.Property(e => e.PaymentMethodId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Status).HasComment("Values: active, inactive, deleted");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("Products_pkey");

            entity.Property(e => e.ProductId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.HasVariant).HasDefaultValue(false);
            entity.Property(e => e.IsTopSeller).HasDefaultValue(false);
            entity
                .Property(e => e.ProductCode)
                .HasDefaultValueSql(
                    "concat('SP', lpad((nextval('product_code_seq'::regclass))::text, 6, '0'::text))"
                );
            entity.Property(e => e.SoldQuantity).HasDefaultValue(0);
            entity.Property(e => e.Status).HasComment("Values: active, inactive, deleted");
            entity.Property(e => e.StockQuantity).HasDefaultValue(0);

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.ProductCreateBies)
                .HasConstraintName("Products_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.ProductModifiedBies)
                .HasConstraintName("Products_modifiedById_fk");
        });

        modelBuilder.Entity<ProductAttribute>(entity =>
        {
            entity.HasKey(e => e.AttributeId).HasName("productAttributes_pk");

            entity.Property(e => e.AttributeId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.Product)
                .WithMany(p => p.ProductAttributes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_productproductattribute");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.ProductCategoryId).HasName("ProductCategories_pkey");

            entity.Property(e => e.ProductCategoryId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.Category)
                .WithMany(p => p.ProductCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_category");

            entity
                .HasOne(d => d.Product)
                .WithMany(p => p.ProductCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_product");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("ProductImages_pkey");

            entity.Property(e => e.ImageId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.Product)
                .WithMany(p => p.ProductImages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_productimage");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.HasKey(e => e.ProductVariantId).HasName("productVariants_pk");

            entity.Property(e => e.ProductVariantId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.SoldQuantity).HasDefaultValue(0);
            entity.Property(e => e.StockQuantity).HasDefaultValue(0);

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.ProductVariantCreateBies)
                .HasConstraintName("ProductVariants_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.ProductVariantModifiedBies)
                .HasConstraintName("ProductVariants_modifiedById_fk");

            entity
                .HasOne(d => d.Product)
                .WithMany(p => p.ProductVariants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("productvariants_products_productid_fk");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("Reviews_pkey");

            entity.Property(e => e.ReviewId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.IsTop).HasDefaultValue(false);
            entity.Property(e => e.Status).HasComment("Values: active, inactive, deleted");

            entity
                .HasOne(d => d.Product)
                .WithMany(p => p.Reviews)
                .HasConstraintName("fk_reviewproduct");

            entity
                .HasOne(d => d.User)
                .WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_reviewuser");
        });

        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.SettingId).HasName("Settings_pkey");

            entity.Property(e => e.SettingId).HasDefaultValueSql("gen_random_uuid()");
            entity
                .Property(e => e.Key)
                .HasComment(
                    "Values: shopName, address, phone, email, maximumTopReview, privacyPolicy, termsOfService, pointsConversionRate"
                );

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.Settings)
                .HasConstraintName("Settings_modifiedById_fk");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("Users_pkey");

            entity.Property(e => e.UserId).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.IsEmailVerified).HasDefaultValue(false);
            entity.Property(e => e.IsPhoneVerified).HasDefaultValue(false);
            entity.Property(e => e.NumberOfOrders).HasDefaultValue(0);
            entity.Property(e => e.TotalSpent).HasDefaultValue(0);
            entity.Property(e => e.Point).HasDefaultValue(0);
            entity.Property(e => e.Status).HasComment("Values: active, inactive, suspended");
            entity
                .Property(e => e.NumberOfOrders)
                .HasComment("Total number of orders placed by the user");
            entity.Property(e => e.TotalSpent).HasComment("Total amount spent by the user");

            entity
                .HasOne(d => d.CreateBy)
                .WithMany(p => p.InverseCreateBy)
                .HasConstraintName("Users_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.InverseModifiedBy)
                .HasConstraintName("Users_modifiedById_fk");
        });

        modelBuilder.Entity<VariantAttribute>(entity =>
        {
            entity.HasKey(e => e.AttributeId).HasName("VariantAttributes_pkey");

            entity.Property(e => e.AttributeId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.ProductVariant)
                .WithMany(p => p.VariantAttributes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("VariantAttributes_productVariantId_fkey");

            entity
                .HasOne(d => d.VariantName)
                .WithMany(p => p.VariantAttributes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("variantAttributes_variantNames_variantNameId_fk");
        });

        modelBuilder.Entity<VariantName>(entity =>
        {
            entity.HasKey(e => e.VariantNameId).HasName("variantAttributes_pk");

            entity.Property(e => e.VariantNameId).HasDefaultValueSql("gen_random_uuid()");

            entity
                .HasOne(d => d.CreatedBy)
                .WithMany(p => p.VariantNameCreatedBies)
                .HasConstraintName("VariantAttributes_createById_fk");

            entity
                .HasOne(d => d.ModifiedBy)
                .WithMany(p => p.VariantNameModifiedBies)
                .HasConstraintName("VariantAttributes_modifyById_fk");
        });
        modelBuilder.HasSequence("order_code_seq").HasMax(999999L);
        modelBuilder.HasSequence("product_code_seq").StartsAt(4L);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
