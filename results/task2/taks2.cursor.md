---
title: "NotificationService API"
version: "1.0.0"
date: "2025-07-02"
---

# Overview
A service for sending notifications via Email, SMS, and Push using SMTP, Twilio, and Firebase.

# API Reference
| Method                                         | Description                                 | Return Type                |
|------------------------------------------------|---------------------------------------------|----------------------------|
| `SendEmailAsync(to, subject, body)`            | Sends an email to the specified address.    | `Task<NotificationResult>` |
| `SendSmsAsync(phoneNumber, message)`           | Sends an SMS to the specified phone number. | `Task<NotificationResult>` |
| `SendPushAsync(deviceToken, title, body)`      | Sends a push notification to a device.      | `Task<NotificationResult>` |

# Examples

## C# Usage
```csharp
// Send Email
var emailResult = await notificationService.SendEmailAsync(
    "user@example.com",
    "Welcome!",
    "Thank you for signing up."
);
if (emailResult.IsSuccess)
{
    Console.WriteLine("Email sent!");
}

// Send SMS
var smsResult = await notificationService.SendSmsAsync(
    "+1234567890",
    "Your code is 1234."
);
if (smsResult.IsSuccess)
{
    Console.WriteLine("SMS sent!");
}

// Send Push Notification
var pushResult = await notificationService.SendPushAsync(
    "device_token_here",
    "New Alert",
    "You have a new message."
);
if (pushResult.IsSuccess)
{
    Console.WriteLine("Push sent!");
}
```

## API Example (cURL)
```bash
curl -X POST https://api.myapp.com/notify/email \
  -H "Content-Type: application/json" \
  -d '{"to":"user@example.com","subject":"Hello","body":"Welcome!"}'

curl -X POST https://api.myapp.com/notify/sms \
  -H "Content-Type: application/json" \
  -d '{"phoneNumber":"+1234567890","message":"Your code is 1234."}'

curl -X POST https://api.myapp.com/notify/push \
  -H "Content-Type: application/json" \
  -d '{"deviceToken":"device_token_here","title":"New Alert","body":"You have a new message."}'
```

### Expected JSON Response
```json
{
  "isSuccess": true,
  "errorCode": null,
  "errorMessage": null
}
```

# Error Codes
| Code         | Description                        |
|--------------|------------------------------------|
| EmailError   | Failed to send email               |
| SmsError     | Failed to send SMS                 |
| PushError    | Failed to send push notification   |

# Notes
- YAML‑front‑matter: metadata  
- Sections in order: Overview, API Reference, Examples, Error Codes, Models  
- Tables: API Reference and Error Codes  
- Code blocks: `csharp`, `bash`, and `json`  
