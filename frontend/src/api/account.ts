import type { SignInPayload, UpdateProfilePayload } from "@/types/account";
import type { Session } from "@/types/sessions";
import type { User } from "@/types/users";
import { get, patch, post } from ".";

export async function getProfile(): Promise<User> {
  return (await get<User>("/api/account/profile")).data;
}

export async function saveProfile(payload: UpdateProfilePayload): Promise<User> {
  return (await patch<UpdateProfilePayload, User>("/api/account/profile", payload)).data;
}

export async function signIn(payload: SignInPayload): Promise<Session> {
  return (await post<SignInPayload, Session>("/api/account/sign/in", payload)).data;
}

export async function signOut(): Promise<void> {
  await post("/api/account/sign/out");
}
