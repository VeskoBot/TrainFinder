variable "rg_name" {
  description = "Name of the resource group"
  type        = string
}

variable "location" {
  description = "Azure region where the resources will be deployed"
  type        = string
  default = "norwayeast"
}

variable "log_analytics_workspace_name" {
  description = "Name of the Log Analytics workspace"
  type        = string
}

variable "sku" {
  description = "SKU (pricing tier) of the Log Analytics workspace"
  type        = string
}

variable "log_analytics_retention_in_days" {
  description = "Retention period in days for the Log Analytics workspace"
  type        = number
}

variable "tags" {
  description = "Tags to apply to the Log Analytics workspace"
  type        = map(string)
}
