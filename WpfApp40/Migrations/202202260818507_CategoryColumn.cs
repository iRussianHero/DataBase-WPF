namespace WpfApp40.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CategoryColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Category", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Category");
        }
    }
}
