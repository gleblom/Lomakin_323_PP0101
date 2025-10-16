using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = Supabase.Postgrest.Attributes.TableAttribute;

namespace DocumentManagementService.Models
{
    [Table("categories")]
    public class Category: BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }

        [Column("category")]
        public string CategoryName {  get; set; }
    }
}
