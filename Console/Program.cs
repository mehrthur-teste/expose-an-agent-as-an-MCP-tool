using System;
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;
using Microsoft.Extensions.DependencyInjection;
using Azure;
using OpenAI;


string endpoint = "";
string apiKey = "";
string deploymentName = "gpt-4.1-mini";

// Cria o agente que vai ser exposto como MCP tool
AIAgent agent = new AzureOpenAIClient(
        new Uri(endpoint),
        new AzureKeyCredential(apiKey))
    .GetChatClient(deploymentName)
    .CreateAIAgent(
        instructions: "You are good at telling jokes.",
        name: "Joker");



// Transforma o agent em MCP tool
McpServerTool tool = McpServerTool.Create(agent.AsAIFunction());

// Sobe o MCP server via stdio
HostApplicationBuilder builder = Host.CreateEmptyApplicationBuilder(settings: null);
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools([tool]);

await builder.Build().RunAsync();
