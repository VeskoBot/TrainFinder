variable "rg_name" {
  type = string
}

variable "location" {
  type = string
}

variable "plan_name" {
  type = string
}

variable "app_name" {
  type = string
}

variable "sku_name" {
  type    = string
  default = "F1"
}

variable "tags" {
  type    = map(string)
  default = {}
}

variable "key_vault_uri" {
  type = string
}
