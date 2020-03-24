namespace Dojo.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Armes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Degats = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ArtMartials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Samourai_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Samourais", t => t.Samourai_Id)
                .Index(t => t.Samourai_Id);
            
            CreateTable(
                "dbo.Samourais",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Force = c.Int(nullable: false),
                        Nom = c.String(),
                        Arme_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Armes", t => t.Arme_Id)
                .Index(t => t.Arme_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ArtMartials", "Samourai_Id", "dbo.Samourais");
            DropForeignKey("dbo.Samourais", "Arme_Id", "dbo.Armes");
            DropIndex("dbo.Samourais", new[] { "Arme_Id" });
            DropIndex("dbo.ArtMartials", new[] { "Samourai_Id" });
            DropTable("dbo.Samourais");
            DropTable("dbo.ArtMartials");
            DropTable("dbo.Armes");
        }
    }
}
