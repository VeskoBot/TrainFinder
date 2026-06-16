
variable "storage_accounts" {
  description = "A map of storage accounts to create"
  type = map(object({
    name                          = string
    sku                           = string
    replication_type              = string
    public_network_access_enabled = bool
  }))
}
variable "rg_name" {
  description = "The Name which should be used for this Resource Group"
  type        = string
  default     = "rg-management-001"
}

variable "tags" {
  description = "A mapping of tags which should be assigned to the Resource Group"
  type        = map(string)
  default = {
    environment = "management"
  }
}

variable "la_workspace" {
  description = "Log Analytics workspace configuration"
  type = object({
    name              = string
    sku               = string
    retention_in_days = number
  })
}