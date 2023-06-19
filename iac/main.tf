# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0.2"
    }
  }

  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "${var.application}-=${var.environment}"
  location = var.location
}

# App Service and Plan
resource "azurerm_service_plan" "webapi_appservice_plan" {
  name                = "webapi-appservice-plan"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  os_type             = "Linux"
  sku_name            = "F1"
}

resource "azurerm_linux_web_app" "webapi" {
  name                  = "webapi"
  location              = azurerm_resource_group.rg.location
  resource_group_name   = azurerm_resource_group.rg.name
  service_plan_id       = azurerm_service_plan.webapi_appservice_plan.id
  https_only            = true
  site_config { 
    minimum_tls_version = "1.2"
  }
  depends_on = [ azurerm_service_plan.webapi_appservice_plan ]
}

resource "azurerm_static_site" "webapp" {
  name                = "webapp"
  resource_group_name = azurerm_resource_group.name
  location            = azurerm_resource_group.location
  sku_size           = "Free"
}