import type { CreateTemplatePayload, Template, SearchTemplatesPayload, UpdateTemplatePayload } from "@/types/templates";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, patch, post } from ".";

export async function createTemplate(payload: CreateTemplatePayload): Promise<Template> {
  return (await post<CreateTemplatePayload, Template>("/api/templates", payload)).data;
}

export async function deleteTemplate(id: string): Promise<Template> {
  return (await _delete<Template>(`/api/templates/${id}`)).data;
}

export async function readTemplate(id: string): Promise<Template> {
  return (await get<Template>(`/api/templates/${id}`)).data;
}

const searchTemplatesQuery = `
query($payload: SearchTemplatesPayload!) {
  templates(payload: $payload) {
    results {
      id
      uniqueName
      displayName
      contentType
      updatedBy {
        id
        type
        isDeleted
        displayName
        emailAddress
        pictureUrl
      }
      updatedOn
    }
    total
  }
}
`;
type SearchTemplatesRequest = {
  payload: SearchTemplatesPayload;
};
type SearchTemplatesResponse = {
  templates: SearchResults<Template>;
};
export async function searchTemplates(payload: SearchTemplatesPayload): Promise<SearchResults<Template>> {
  return (await graphQL<SearchTemplatesRequest, SearchTemplatesResponse>(searchTemplatesQuery, { payload })).data.templates;
}

export async function updateTemplate(id: string, payload: UpdateTemplatePayload): Promise<Template> {
  return (await patch<UpdateTemplatePayload, Template>(`/api/templates/${id}`, payload)).data;
}
