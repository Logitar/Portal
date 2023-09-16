import type { InjectionKey } from "vue";

export const bindItemKey = Symbol() as InjectionKey<(id: string, setParentId: (value?: string) => void) => void>;
export const unbindItemKey = Symbol() as InjectionKey<(id: string) => void>;
