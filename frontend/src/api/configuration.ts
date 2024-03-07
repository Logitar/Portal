import type { Configuration, ReplaceConfigurationPayload } from "@/types/configuration";
import { get, put } from ".";

export async function readConfiguration(): Promise<Configuration> {
  return (await get<Configuration>("/api/configuration")).data;
}

export async function replaceConfiguration(payload: ReplaceConfigurationPayload, version?: number): Promise<Configuration> {
  const query: string = version ? `?version=${version}` : "";
  return (await put<ReplaceConfigurationPayload, Configuration>(`/api/configuration${query}`, payload)).data;
}
