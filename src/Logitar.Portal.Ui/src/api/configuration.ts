import type { Configuration, IsConfigurationInitializedResult } from "@/types/configuration";
import type { InitializeConfigurationPayload, UpdateConfigurationPayload } from "@/types/configuration/payloads";
import { get, patch, post } from ".";

export async function initializeConfiguration(payload: InitializeConfigurationPayload): Promise<void> {
  await post<InitializeConfigurationPayload, void>("/api/configuration", payload);
}

export async function isConfigurationInitialized(): Promise<IsConfigurationInitializedResult> {
  return (await get<IsConfigurationInitializedResult>("/api/configuration/initialized")).data;
}

export async function readConfiguration(): Promise<Configuration> {
  return (await get<Configuration>("/api/configuration")).data;
}

export async function updateConfiguration(payload: UpdateConfigurationPayload): Promise<Configuration> {
  return (await patch<UpdateConfigurationPayload, Configuration>("/api/configuration", payload)).data;
}
