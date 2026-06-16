# Purpose: Define the output variables for the Log Analytics workspace module.
output "workspace_resource" {
  value       = one(azurerm_log_analytics_workspace.main[*])
  description = "The ID of the Log Analytics workspace."
}
