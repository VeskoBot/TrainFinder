rg_name     = "rg-trainfinder-dev-001"
location    = "norwayeast"
environment = "dev"

tags = {
  environment = "dev"
  project     = "TrainFinder"
}

# SQL Server + Database (Standard S0 tier ~$16/month)
sql = {
  server_name        = "sql-trainfinder-dev-001"
  db_name            = "sqldb-trainfinder-dev-001"
  sku_name           = "S0"
  admin_login        = "sqladmin"
  max_size_gb        = 30
  geo_backup_enabled = true
}

# Key Vault
key_vault_name = "kv-trainfinder-dev-001"

# Storage Account
storage_account_name = "sattrainfinderdev001"

# App Service (F1 = Free tier)
app_service = {
  plan_name = "asp-trainfinder-dev-001"
  app_name  = "app-trainfinder-dev-001"
  sku_name  = "F1"
}
