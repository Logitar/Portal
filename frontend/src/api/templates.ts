import type { CreateTemplatePayload, ReplaceTemplatePayload, Template, SearchTemplatesPayload } from "@/types/templates";
import type { SearchResults } from "@/types/search";
import { _delete, get, graphQL, post, put } from ".";

export async function createTemplate(payload: CreateTemplatePayload): Promise<Template> {
  return (await post<CreateTemplatePayload, Template>("/api/templates", payload)).data;
}

export async function deleteTemplate(id: string): Promise<Template> {
  return (await _delete<Template>(`/api/templates/${id}`)).data;
}

const listTemplatesQuery = `
query($payload: SearchTemplatesPayload!) {
  templates(payload: $payload) {
    items {
      id
      uniqueKey
      displayName
      content {
        type
      }
    }
  }
}
`;
type ListTemplatesRequest = {
  payload: SearchTemplatesPayload;
};
type ListTemplatesResponse = {
  templates: SearchResults<Template>;
};
export async function listTemplates(): Promise<Template[]> {
  return (await graphQL<ListTemplatesRequest, ListTemplatesResponse>(listTemplatesQuery, { payload: {} })).data.templates.items;
}

export async function readTemplate(id: string): Promise<Template> {
  return (await get<Template>(`/api/templates/${id}`)).data;
}

export async function replaceTemplate(id: string, payload: ReplaceTemplatePayload, version?: number): Promise<Template> {
  const query: string = version ? `?version=${version}` : "";
  return (await put<ReplaceTemplatePayload, Template>(`/api/templates/${id}${query}`, payload)).data;
}

const searchTemplatesQuery = `
query($payload: SearchTemplatesPayload!) {
  templates(payload: $payload) {
    items {
      id
      uniqueKey
      displayName
      content {
        type
      }
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
