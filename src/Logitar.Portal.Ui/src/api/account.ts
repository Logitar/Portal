import type { ChangePasswordPayload, SaveContactInformationPayload, SavePersonalInformationPayload, SignInPayload } from "@/types/users/payloads";
import type { UserProfile } from "@/types/users";
import { get, post, put } from ".";

export async function changePassword(payload: ChangePasswordPayload): Promise<UserProfile> {
  return (await post<ChangePasswordPayload, UserProfile>("/account/password/change", payload)).data;
}

export async function getProfile(): Promise<UserProfile> {
  return (await get<UserProfile>("/account/profile")).data;
}

export async function saveContactInformation(payload: SaveContactInformationPayload): Promise<UserProfile> {
  return (await put<SaveContactInformationPayload, UserProfile>("/account/profile/contact", payload)).data;
}

export async function savePersonalInformation(payload: SavePersonalInformationPayload): Promise<UserProfile> {
  return (await put<SavePersonalInformationPayload, UserProfile>("/account/profile/personal", payload)).data;
}

export async function signIn(payload: SignInPayload): Promise<UserProfile> {
  return (await post<SignInPayload, UserProfile>("/account/sign/in", payload)).data;
}

export async function signOut(): Promise<void> {
  await post("/account/sign/out");
}
