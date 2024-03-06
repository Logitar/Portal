import type {
  Configuration,
  InitializeConfigurationPayload,
  InitializeConfigurationResult,
  IsConfigurationInitializedResult,
  UpdateConfigurationPayload,
} from "@/types/configuration";
import { get, patch, post } from ".";

export async function initializeConfiguration(payload: InitializeConfigurationPayload): Promise<InitializeConfigurationResult> {
  return (await post<InitializeConfigurationPayload, InitializeConfigurationResult>("/api/configuration", payload)).data;
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
