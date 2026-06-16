output "server_id" {
  value = azurerm_mssql_server.main.id
}

output "server_fqdn" {
  value = azurerm_mssql_server.main.fully_qualified_domain_name
}

output "db_id" {
  value = azurerm_mssql_database.main.id
}

output "admin_password" {
  value     = random_password.sql_admin.result
  sensitive = true
}
