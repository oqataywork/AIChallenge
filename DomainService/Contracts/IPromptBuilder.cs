using Domain;

namespace DomainService.Contracts;

public interface IPromptBuilder
{
    IPromptBuilder WithUserPrompt(string prompt);
    IPromptBuilder WithTemperature(double temperature);
    IPromptBuilder WithContext(List<AiContext> context);
    IPromptBuilder WithoutContext();
    IPromptBuilder WithCompressionPercentage(int compressionPercentage);
    string Build();
}