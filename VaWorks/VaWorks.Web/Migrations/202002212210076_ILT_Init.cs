namespace VaWorks.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ILT_Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActuatorInterfaceCodes",
                c => new
                    {
                        InterfaceCode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InterfaceCode);
            
            CreateTable(
                "dbo.Actuators",
                c => new
                    {
                        ActuatorId = c.Int(nullable: false, identity: true),
                        InterfaceCode = c.Int(nullable: false),
                        Manufacturer = c.String(nullable: false, maxLength: 25),
                        Model = c.String(nullable: false, maxLength: 25),
                        Size = c.String(nullable: false, maxLength: 25),
                    })
                .PrimaryKey(t => t.ActuatorId)
                .ForeignKey("dbo.ActuatorInterfaceCodes", t => t.InterfaceCode, cascadeDelete: true)
                .Index(t => t.InterfaceCode);

            CreateTable(
                "dbo.Organizations",
                c => new
                {
                    OrganizationId = c.Int(nullable: false, identity: true),
                    ParentId = c.Int(),
                    Name = c.String(),
                    Description = c.String(),
                    Address1 = c.String(),
                    Address2 = c.String(),
                    City = c.String(),
                    State = c.String(),
                    Country = c.String(),
                    PostalCode = c.String(),
                })
                .PrimaryKey(t => t.OrganizationId)
                .ForeignKey("dbo.Organizations", t => t.ParentId)
                .Index(t => t.ParentId);

            CreateTable(
                "dbo.Discounts",
                c => new
                    {
                        DiscountId = c.Int(nullable: false, identity: true),
                        OrganizationId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        DiscountPercentage = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.DiscountId)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .Index(t => t.OrganizationId);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        DocumentId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        FileName = c.String(maxLength: 250),
                        Description = c.String(maxLength: 250),
                        DocumentType = c.Int(nullable: false),
                        FileData = c.Binary(),
                        ContentType = c.String(),
                    })
                .PrimaryKey(t => t.DocumentId);
            
            CreateTable(
                "dbo.Kits",
                c => new
                    {
                        KitId = c.Int(nullable: false, identity: true),
                        KitNumber = c.String(nullable: false, maxLength: 30),
                        ValveInterfaceCode = c.Int(nullable: false),
                        ActuatorInterfaceCode = c.Int(nullable: false),
                        KitMaterialId = c.Int(nullable: false),
                        KitOptionId = c.Int(nullable: false),
                        Price = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.KitId)
                .ForeignKey("dbo.ActuatorInterfaceCodes", t => t.ActuatorInterfaceCode, cascadeDelete: true)
                .ForeignKey("dbo.KitMaterials", t => t.KitMaterialId, cascadeDelete: true)
                .ForeignKey("dbo.KitOptions", t => t.KitOptionId, cascadeDelete: true)
                .ForeignKey("dbo.ValveInterfaceCodes", t => t.ValveInterfaceCode, cascadeDelete: true)
                .Index(t => t.ValveInterfaceCode)
                .Index(t => t.ActuatorInterfaceCode)
                .Index(t => t.KitMaterialId)
                .Index(t => t.KitOptionId);
            
            CreateTable(
                "dbo.KitMaterials",
                c => new
                    {
                        KitMaterialId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Code = c.String(nullable: false, maxLength: 10),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.KitMaterialId);
            
            CreateTable(
                "dbo.KitOptions",
                c => new
                    {
                        KitOptionId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Code = c.String(nullable: false, maxLength: 10),
                        SortOrder = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.KitOptionId);
            
            CreateTable(
                "dbo.ValveInterfaceCodes",
                c => new
                    {
                        InterfaceCode = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InterfaceCode);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        OrganizationId = c.Int(),
                        Name = c.String(maxLength: 100),
                        Title = c.String(),
                        LinkedIn = c.String(),
                        Facebook = c.String(),
                        Twitter = c.String(),
                        Skype = c.String(),
                        ImageString = c.String(),
                        IsSales = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        ShoppingCart_ShoppingCartItemId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organizations", t => t.OrganizationId)
                .ForeignKey("dbo.ShoppingCartItems", t => t.ShoppingCart_ShoppingCartItemId, cascadeDelete: true)
                .Index(t => t.OrganizationId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.ShoppingCart_ShoppingCartItemId);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.SystemMessages",
                c => new
                    {
                        UserMessageId = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        DateSent = c.DateTimeOffset(nullable: false, precision: 7),
                        IsRead = c.Boolean(nullable: false),
                        DateRead = c.DateTimeOffset(precision: 7),
                        MessagePreview = c.String(maxLength: 100),
                        Message = c.String(),
                    })
                .PrimaryKey(t => t.UserMessageId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Quotes",
                c => new
                    {
                        QuoteId = c.Int(nullable: false, identity: true),
                        CreatedById = c.String(nullable: false, maxLength: 128),
                        CustomerId = c.String(maxLength: 128),
                        OrganizationId = c.Int(nullable: false),
                        QuoteNumber = c.Int(nullable: false),
                        Revision = c.String(maxLength: 5),
                        CreatedByName = c.String(nullable: false),
                        CustomerName = c.String(nullable: false),
                        CompanyName = c.String(nullable: false),
                        Address1 = c.String(),
                        Address2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        PostalCode = c.String(),
                        SalesPerson = c.String(nullable: false),
                        CreatedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        ModifiedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        OrderDate = c.DateTimeOffset(nullable: false, precision: 7),
                        Title = c.String(maxLength: 100),
                        Total = c.Double(nullable: false),
                        IsSent = c.Boolean(nullable: false),
                        IsOrder = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.QuoteId)
                .ForeignKey("dbo.AspNetUsers", t => t.CustomerId)
                .ForeignKey("dbo.AspNetUsers", t => t.CreatedById, cascadeDelete: true)
                .Index(t => t.CreatedById)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.QuoteItems",
                c => new
                    {
                        QuoteItemId = c.Int(nullable: false, identity: true),
                        QuoteId = c.Int(nullable: false),
                        KitNumber = c.String(maxLength: 100),
                        Valve = c.String(maxLength: 100),
                        Actuator = c.String(maxLength: 100),
                        Description = c.String(maxLength: 100),
                        Quantity = c.Int(nullable: false),
                        PriceEach = c.Double(nullable: false),
                        Discount = c.Double(nullable: false),
                        TotalPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.QuoteItemId)
                .ForeignKey("dbo.Quotes", t => t.QuoteId, cascadeDelete: true)
                .Index(t => t.QuoteId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ShoppingCartItems",
                c => new
                    {
                        ShoppingCartItemId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        KitId = c.Int(),
                        ValveId = c.Int(),
                        ActuatorId = c.Int(),
                        Quantity = c.Int(nullable: false),
                        QuoteNumber = c.Int(),
                        Revision = c.String(),
                    })
                .PrimaryKey(t => t.ShoppingCartItemId)
                .ForeignKey("dbo.Actuators", t => t.ActuatorId)
                .ForeignKey("dbo.Kits", t => t.KitId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.Valves", t => t.ValveId)
                .Index(t => t.UserId)
                .Index(t => t.KitId)
                .Index(t => t.ValveId)
                .Index(t => t.ActuatorId);
            
            CreateTable(
                "dbo.Valves",
                c => new
                    {
                        ValveId = c.Int(nullable: false, identity: true),
                        InterfaceCode = c.Int(nullable: false),
                        Manufacturer = c.String(nullable: false, maxLength: 25),
                        Model = c.String(nullable: false, maxLength: 25),
                        Size = c.String(nullable: false, maxLength: 25),
                    })
                .PrimaryKey(t => t.ValveId)
                .ForeignKey("dbo.ValveInterfaceCodes", t => t.InterfaceCode, cascadeDelete: true)
                .Index(t => t.InterfaceCode);
            
            CreateTable(
                "dbo.InvitationRequests",
                c => new
                    {
                        InvitationRequestId = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false),
                        Name = c.String(nullable: false),
                        Company = c.String(nullable: false),
                        RequestDate = c.DateTimeOffset(nullable: false, precision: 7),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.InvitationRequestId);
            
            CreateTable(
                "dbo.Invitations",
                c => new
                    {
                        InvitationId = c.Int(nullable: false, identity: true),
                        OrganizationId = c.Int(nullable: false),
                        Name = c.String(maxLength: 250),
                        Company = c.String(nullable: false, maxLength: 250),
                        Email = c.String(nullable: false),
                        SalesPersonEmail = c.String(),
                        Type = c.Int(nullable: false),
                        CreatedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        SentDate = c.DateTimeOffset(nullable: false, precision: 7),
                        IsClaimed = c.Boolean(nullable: false),
                        ClaimedDate = c.DateTimeOffset(nullable: false, precision: 7),
                        ClaimedEmail = c.String(),
                    })
                .PrimaryKey(t => t.InvitationId);
            
            CreateTable(
                "dbo.QuoteNumber",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.OrganizationActuators",
                c => new
                    {
                        OrganizationId = c.Int(nullable: false),
                        ActuatorId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganizationId, t.ActuatorId })
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Actuators", t => t.ActuatorId, cascadeDelete: true)
                .Index(t => t.OrganizationId)
                .Index(t => t.ActuatorId);
            
            CreateTable(
                "dbo.OrganizationDocuments",
                c => new
                    {
                        OrganizationId = c.Int(nullable: false),
                        DocumentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganizationId, t.DocumentId })
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Documents", t => t.DocumentId, cascadeDelete: true)
                .Index(t => t.OrganizationId)
                .Index(t => t.DocumentId);
            
            CreateTable(
                "dbo.OrganizationKits",
                c => new
                    {
                        OrganizationId = c.Int(nullable: false),
                        KitId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganizationId, t.KitId })
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Kits", t => t.KitId, cascadeDelete: true)
                .Index(t => t.OrganizationId)
                .Index(t => t.KitId);
            
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        ContactId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.ContactId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.ContactId)
                .Index(t => t.UserId)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.OrganizationValves",
                c => new
                    {
                        OrganizationId = c.Int(nullable: false),
                        ValveId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrganizationId, t.ValveId })
                .ForeignKey("dbo.Organizations", t => t.OrganizationId, cascadeDelete: true)
                .ForeignKey("dbo.Valves", t => t.ValveId, cascadeDelete: true)
                .Index(t => t.OrganizationId)
                .Index(t => t.ValveId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.OrganizationValves", "ValveId", "dbo.Valves");
            DropForeignKey("dbo.OrganizationValves", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.AspNetUsers", "ShoppingCart_ShoppingCartItemId", "dbo.ShoppingCartItems");
            DropForeignKey("dbo.ShoppingCartItems", "ValveId", "dbo.Valves");
            DropForeignKey("dbo.Valves", "InterfaceCode", "dbo.ValveInterfaceCodes");
            DropForeignKey("dbo.ShoppingCartItems", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ShoppingCartItems", "KitId", "dbo.Kits");
            DropForeignKey("dbo.ShoppingCartItems", "ActuatorId", "dbo.Actuators");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Quotes", "CreatedById", "dbo.AspNetUsers");
            DropForeignKey("dbo.Quotes", "CustomerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.QuoteItems", "QuoteId", "dbo.Quotes");
            DropForeignKey("dbo.AspNetUsers", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.SystemMessages", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Contacts", "ContactId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Contacts", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganizationKits", "KitId", "dbo.Kits");
            DropForeignKey("dbo.OrganizationKits", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Kits", "ValveInterfaceCode", "dbo.ValveInterfaceCodes");
            DropForeignKey("dbo.Kits", "KitOptionId", "dbo.KitOptions");
            DropForeignKey("dbo.Kits", "KitMaterialId", "dbo.KitMaterials");
            DropForeignKey("dbo.Kits", "ActuatorInterfaceCode", "dbo.ActuatorInterfaceCodes");
            DropForeignKey("dbo.OrganizationDocuments", "DocumentId", "dbo.Documents");
            DropForeignKey("dbo.OrganizationDocuments", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Discounts", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Organizations", "ParentId", "dbo.Organizations");
            DropForeignKey("dbo.OrganizationActuators", "ActuatorId", "dbo.Actuators");
            DropForeignKey("dbo.OrganizationActuators", "OrganizationId", "dbo.Organizations");
            DropForeignKey("dbo.Actuators", "InterfaceCode", "dbo.ActuatorInterfaceCodes");
            DropIndex("dbo.OrganizationValves", new[] { "ValveId" });
            DropIndex("dbo.OrganizationValves", new[] { "OrganizationId" });
            DropIndex("dbo.Contacts", new[] { "ContactId" });
            DropIndex("dbo.Contacts", new[] { "UserId" });
            DropIndex("dbo.OrganizationKits", new[] { "KitId" });
            DropIndex("dbo.OrganizationKits", new[] { "OrganizationId" });
            DropIndex("dbo.OrganizationDocuments", new[] { "DocumentId" });
            DropIndex("dbo.OrganizationDocuments", new[] { "OrganizationId" });
            DropIndex("dbo.OrganizationActuators", new[] { "ActuatorId" });
            DropIndex("dbo.OrganizationActuators", new[] { "OrganizationId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Valves", new[] { "InterfaceCode" });
            DropIndex("dbo.ShoppingCartItems", new[] { "ActuatorId" });
            DropIndex("dbo.ShoppingCartItems", new[] { "ValveId" });
            DropIndex("dbo.ShoppingCartItems", new[] { "KitId" });
            DropIndex("dbo.ShoppingCartItems", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.QuoteItems", new[] { "QuoteId" });
            DropIndex("dbo.Quotes", new[] { "CustomerId" });
            DropIndex("dbo.Quotes", new[] { "CreatedById" });
            DropIndex("dbo.SystemMessages", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "ShoppingCart_ShoppingCartItemId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "OrganizationId" });
            DropIndex("dbo.Kits", new[] { "KitOptionId" });
            DropIndex("dbo.Kits", new[] { "KitMaterialId" });
            DropIndex("dbo.Kits", new[] { "ActuatorInterfaceCode" });
            DropIndex("dbo.Kits", new[] { "ValveInterfaceCode" });
            DropIndex("dbo.Discounts", new[] { "OrganizationId" });
            DropIndex("dbo.Organizations", new[] { "ParentId" });
            DropIndex("dbo.Actuators", new[] { "InterfaceCode" });
            DropTable("dbo.OrganizationValves");
            DropTable("dbo.Contacts");
            DropTable("dbo.OrganizationKits");
            DropTable("dbo.OrganizationDocuments");
            DropTable("dbo.OrganizationActuators");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.QuoteNumber");
            DropTable("dbo.Invitations");
            DropTable("dbo.InvitationRequests");
            DropTable("dbo.Valves");
            DropTable("dbo.ShoppingCartItems");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.QuoteItems");
            DropTable("dbo.Quotes");
            DropTable("dbo.SystemMessages");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ValveInterfaceCodes");
            DropTable("dbo.KitOptions");
            DropTable("dbo.KitMaterials");
            DropTable("dbo.Kits");
            DropTable("dbo.Documents");
            DropTable("dbo.Discounts");
            DropTable("dbo.Organizations");
            DropTable("dbo.Actuators");
            DropTable("dbo.ActuatorInterfaceCodes");
        }
    }
}
