import type { SenderProvider, SenderType } from "@/types/senders";

export function getSenderType(provider: SenderProvider): SenderType | undefined {
  switch (provider) {
    case "Mailgun":
    case "SendGrid":
      return "Email";
    case "Twilio":
      return "Sms";
  }
  return undefined;
}
