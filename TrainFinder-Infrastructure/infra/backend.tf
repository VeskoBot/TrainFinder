terraform {
  backend "azurerm" {
    storage_account_name = "overwritten by Azure DevOps"
    container_name       = "overwritten by Azure DevOps"
    key                  = "overwritten by Azure DevOps"
  }
}
