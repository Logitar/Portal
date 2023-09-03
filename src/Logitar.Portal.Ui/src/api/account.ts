import type { SignInPayload, UpdateUserPayload } from "@/types/users/payloads";
import type { User } from "@/types/users";
import { get, post, patch } from ".";

export async function getProfile(): Promise<User> {
  return (await get<User>("/account/profile")).data;
}

export async function saveProfile(payload: UpdateUserPayload): Promise<User> {
  return (await patch<UpdateUserPayload, User>("/account/profile", payload)).data;
}

export async function signIn(payload: SignInPayload): Promise<User> {
  return (await post<SignInPayload, User>("/account/sign/in", payload)).data;
}

export async function signOut(): Promise<void> {
  await post("/account/sign/out");
}
