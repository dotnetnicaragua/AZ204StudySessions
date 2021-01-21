using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;

namespace DemoCh11
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            var credentials = SdkContext.AzureCredentialsFactory.FromFile("../../../credentials.properties");

            var azure = Azure.Configure()
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(credentials)
                .WithDefaultSubscription();

            //Define "resources" names
            var groupName = "az-dotNetNicaragua";
            var vmname = "demoChapter11";
            var location = Region.USEast;
            
            var virtualNetworkName = "vNetworkDemo";
            var virtualNetworkAddress = "192.168.0.0/16";
            var virtualSubNetworkName = "vSubNetworkDemo";
            var subNetName = "192.168.0.0/24";

            var nicName = "demoChapter11";
            var adminUser = "DotNetNicaragua";
            var adminPassword = "P4st0R+-";
            var publicIp = "DotNetNicaragua-ip";
            var nsgName = "azDemo-NSG";

            Console.WriteLine($"Creating Resource Group: {groupName}");
            var rGroup = azure.ResourceGroups.Define(groupName)
                .WithRegion(location)
                .Create();

            Console.WriteLine($"Creating Network: {virtualNetworkName} and SubNetWork: {virtualSubNetworkName}");
            var network = azure.Networks.Define(virtualNetworkName)
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithAddressSpace(virtualNetworkAddress)
                .WithSubnet(virtualSubNetworkName, subNetName)
                .Create();

            Console.WriteLine($"Creating Public IP: {publicIp}");
            var publicIP = azure.PublicIPAddresses.Define(publicIp)
                .WithRegion(location)
                .WithExistingResourceGroup(groupName);

            Console.WriteLine($"Creating Network Security Group: {nsgName}");
            var nsgGroup = azure.NetworkSecurityGroups.Define(nsgName)
                .WithRegion(location)
                .WithNewResourceGroup(groupName);

            nsgGroup.DefineRule("Allow-RDP")
                .AllowInbound()
                .FromAnyAddress()
                .FromAnyPort()
                .ToAnyAddress()
                .ToPort(3000)
                .WithProtocol(SecurityRuleProtocol.Tcp)
                .WithPriority(100)
                .WithDescription("Allowing RDP")
                .Attach();

            Console.WriteLine($"Creating Network Interface: {nicName}");
            var networkInterface = azure.NetworkInterfaces.Define(nicName)
                .WithRegion(location)
                .WithExistingResourceGroup(groupName)
                .WithExistingPrimaryNetwork(network)
                .WithSubnet(virtualSubNetworkName)
                .WithPrimaryPrivateIPAddressDynamic()
                .WithNewPrimaryPublicIPAddress(publicIp)
                .WithNewNetworkSecurityGroup(nsgGroup)
                .Create();

            Console.WriteLine($"Creating the Virtual Machine: {vmname}");
            azure.VirtualMachines.Define(vmname)
                .WithRegion(location)
                .WithNewResourceGroup(groupName)
                .WithExistingPrimaryNetworkInterface(networkInterface)
                .WithLatestWindowsImage("MicrosoftWindowsServer", "WindowsServer", "2012-R2-Datacenter")
                .WithAdminUsername(adminUser)
                .WithAdminPassword(adminPassword)
                .WithComputerName(vmname)
                .WithSize(VirtualMachineSizeTypes.BasicA4)
                .Create();



        }
    }
}
