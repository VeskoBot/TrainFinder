output "app_id" {
  value = azurerm_windows_web_app.main.id
}

output "app_name" {
  value = azurerm_windows_web_app.main.name
}

output "default_hostname" {
  value = azurerm_windows_web_app.main.default_hostname
}

output "plan_id" {
  value = azurerm_service_plan.main.id
}

output "principal_id" {
  value = azurerm_windows_web_app.main.identity[0].principal_id
}
