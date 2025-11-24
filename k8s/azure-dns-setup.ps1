# Get the public IP and add a DNS label

# 1. First, get the public IP resource created by the LoadBalancer
$RESOURCE_GROUP = "MC_<your-rg>_<your-aks-cluster>_<region>"  # This is the managed resource group created by AKS
$PUBLIC_IP_NAME = "<your-public-ip-name>"  # Find this in the Azure Portal or using the command below

# Find the public IP
az network public-ip list --resource-group $RESOURCE_GROUP --output table

# 2. Add a DNS label to get a free subdomain
az network public-ip update `
  --resource-group $RESOURCE_GROUP `
  --name $PUBLIC_IP_NAME `
  --dns-name paytrackr

# This gives you: paytrackr.<region>.cloudapp.azure.com
