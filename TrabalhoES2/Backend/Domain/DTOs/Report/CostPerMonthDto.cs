namespace Backend.Domain.DTOs.Report;

public record CostPerMonthDto  (int Month, int Year, decimal TotalCost);