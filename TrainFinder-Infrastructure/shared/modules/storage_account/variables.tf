###### global variable variables ######

variable "rg_name" {
  description = "the name of resource group"
  type        = string
}

variable "location" {
  description = "the location of resource group"
  type        = string
  default     = "norwayeast"
}

variable "storage_account_name" {
  description = "The name of the storage account"
  type        = string
}

variable "storage_account_sku" {
  description = "SKU (pricing tier) of the storage account"
  type        = string
}

variable "tags" {
  description = "Tags to apply to the storage account"
  type        = map(string)
}

variable "storage_account_replication_type" {
  description = "The kind of the storage account"
  type        = string
}

variable "public_network_access_enabled" {
  description = "Controls whether data in the storage account may be accessed from public networks"
  type        = bool
}