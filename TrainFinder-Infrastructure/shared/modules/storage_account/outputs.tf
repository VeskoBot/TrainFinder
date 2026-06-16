output "storage_account_resource" {
  value       = one(azurerm_storage_account.storage_account[*])
  description = "The Storage Account resource."
}
