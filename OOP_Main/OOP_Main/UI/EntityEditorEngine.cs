using OOP_Main.Attributes;
using Spectre.Console;
using System;
using System.Linq;
using System.Reflection;

namespace OOP_Main.UI {
    public class EntityEditorEngine: UIHelper {
        public static void EditEntity(object target) {
            if (target == null) return;

            // 1. Якщо товар задекорований (наприклад, DiscountDecorator), даємо вибір
            if (target is ProductDecorator decorator && decorator.TargetProduct.HasValue()) {
                var layerChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("What exactly do you want to edit?")
                        .AddChoices("Decorator parameters (discount/warranty)", "Base product")
                );

                if (layerChoice == "Base product") {
                    target = decorator.TargetProduct.Value; // Переходимо до Cloth, Sofa тощо
                }
            }

            // 2. Беремо всі властивості об'єкта, які мають наш атрибут [Editable]
            var editableProperties = target.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(p => new {
                    Property = p,
                    Attribute = p.GetCustomAttribute<EditableAttribute>()
                })
                .Where(x => x.Attribute != null && x.Property.CanWrite)
                .ToList();

            if (!editableProperties.Any()) {
                DefaultErrorOutput("This object has no editable fields.");
                return;
            }

            // 3. Формуємо список для вибору поля у консолі
            var propToEdit = AnsiConsole.Prompt(
                new SelectionPrompt<dynamic>()
                    .Title("Choose field to edit:")
                    .UseConverter(x => {
                        var rawValue = x.Property.GetValue(target);

                        string safeValue = Markup.Escape(rawValue?.ToString() ?? "null");
                        string safeName = Markup.Escape(x.Attribute.DisplayName);

                        return $"{safeName} [yellow](current: {safeValue})[/]";
                    })
                    .AddChoices(editableProperties)
            );

            PropertyInfo prop = propToEdit.Property;
            Type propType = prop.PropertyType;

            // 4. Запитуємо нове значення залежно від типу поля
            try {
                if (propType == typeof(string)) {
                    string newValue = DefaultTextPrompt<string>($"Enter a new value for {propToEdit.Attribute.DisplayName}:");
                    prop.SetValue(target, newValue);
                }
                else if (propType == typeof(double)) {
                    double newValue = DefaultTextPrompt<double>($"Enter a new value for {propToEdit.Attribute.DisplayName}:");
                    prop.SetValue(target, newValue);
                }
                else if (propType == typeof(int)) {
                    int newValue = DefaultTextPrompt<int>($"Enter a new value for {propToEdit.Attribute.DisplayName}:");
                    prop.SetValue(target, newValue);
                }
                else if (propType == typeof(bool)) {
                    bool newValue = DefaultConfirm($"Switch {propToEdit.Attribute.DisplayName}?");
                    prop.SetValue(target, newValue);
                }
                else if (propType == typeof(OrderStatus)) { 
                    OrderStatus newValue = AnsiConsole.Prompt(
                        new SelectionPrompt<OrderStatus>()
                            .Title($"Enter a new value for {propToEdit.Attribute.DisplayName}:")
                            .UseConverter(stauts => OrderStatusExtensions.GetDescription(stauts))
                            .AddChoices(OrderStatusExtensions.GetOrderStatuses())
                    );
                    prop.SetValue(target, newValue);
                }

                    DefaultSuccessOutput("Field is successfully updated!");
            }
            catch (TargetInvocationException ex) {
                // Відловлюємо помилки з валідаторів у ваших сеттерах (наприклад, від'ємна ціна)
                DefaultErrorOutput($"Validation error: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex) {
                DefaultErrorOutput($"Failed to change the value: {ex.Message}");
            }
        }
    }
}