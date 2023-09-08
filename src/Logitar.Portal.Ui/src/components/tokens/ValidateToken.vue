<script setup lang="ts">
import { inject, onUpdated, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import EmailAddressInput from "../users/EmailAddressInput.vue";
import FormInput from "@/components/shared/FormInput.vue";
import RealmSelect from "@/components/realms/RealmSelect.vue";
import type { Claim, ValidatedToken } from "@/types/tokens";
import type { Realm } from "@/types/realms";
import { registerTooltipsKey } from "@/inject/App";
import { validateToken } from "@/api/tokens";

const registerTooltips = inject(registerTooltipsKey) as () => void;
const { d, t } = useI18n();

const audience = ref<string>("");
const consume = ref<boolean>(false);
const issuer = ref<string>("");
const purpose = ref<string>("");
const realm = ref<Realm>();
const secret = ref<string>("");
const token = ref<string>("");
const validatedToken = ref<ValidatedToken>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

function formatClaimType(type: string): string {
  const index = type.indexOf("#");
  return index < 0 ? type : type.substring(index + 1);
}
function formatDateTime(claim: Claim): string {
  return d(new Date(Number(claim.value) * 1000), "medium");
}
function isDateTimeClaim(claim: Claim): boolean {
  return Boolean(claim.type && formatClaimType(claim.type).toLowerCase() === "integer" && !Number.isNaN(claim.value));
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    validatedToken.value = await validateToken({
      token: token.value,
      consume: consume.value,
      purpose: purpose.value,
      realm: realm.value?.id,
      audience: audience.value,
      issuer: issuer.value,
      secret: secret.value,
    });
  } catch (e: unknown) {
    emit("error", e);
  }
});

onUpdated(() => registerTooltips());
</script>

<template>
  <div>
    <form @submit.prevent="onSubmit">
      <div class="mb-3">
        <icon-submit :disabled="isSubmitting" icon="fas fa-id-card" :loading="isSubmitting" text="actions.validate" />
      </div>
      <FormInput id="token" label="tokens.input.label" placeholder="tokens.input.placeholder" required v-model="token" />
      <div class="row">
        <RealmSelect class="col-lg-6" :model-value="realm?.id" @realm-selected="realm = $event" />
        <FormInput
          class="col-lg-6"
          id="secret"
          label="tokens.secret.label"
          :min-length="32"
          :max-length="64"
          placeholder="tokens.secret.placeholder"
          v-model="secret"
        />
      </div>
      <form-checkbox class="mb-2" id="consume" label="tokens.consume" v-model="consume" />
      <FormInput id="purpose" label="tokens.purpose.label" placeholder="tokens.purpose.placeholder" v-model="purpose" />
      <div class="row">
        <FormInput class="col-lg-6" id="audience" label="tokens.audience.label" placeholder="tokens.audience.placeholder" v-model="audience" />
        <FormInput class="col-lg-6" id="issuer" label="tokens.issuer.label" placeholder="tokens.issuer.placeholder" v-model="issuer" />
      </div>
    </form>
    <template v-if="validatedToken">
      <h3>{{ t("tokens.validated.title") }}</h3>
      <div class="row">
        <EmailAddressInput class="col-lg-6" disabled :model-value="validatedToken.emailAddress" />
        <FormInput class="col-lg-6" id="subject" label="tokens.subject.label" placeholder="tokens.subject.placeholder" :model-value="validatedToken.subject" />
      </div>
      <h5>{{ t("tokens.claims.title") }}</h5>
      <table v-if="validatedToken.claims.length" class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("tokens.claims.name.label") }}</th>
            <th scope="col">{{ t("tokens.claims.value.label") }}</th>
            <th scope="col">{{ t("tokens.claims.type.label") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(claim, index) in validatedToken.claims" :key="index">
            <td>{{ claim.name }}</td>
            <td>
              <template v-if="isDateTimeClaim(claim)">
                <span data-bs-toggle="tooltip" :data-bs-title="formatDateTime(claim)"> <font-awesome-icon icon="fas fa-circle-info" /> {{ claim.value }} </span>
              </template>
              <template v-else>{{ claim.value }}</template>
            </td>
            <td>
              <template v-if="claim.type">{{ formatClaimType(claim.type) }}</template>
            </td>
          </tr>
        </tbody>
      </table>
      <p v-else>{{ t("tokens.claims.empty") }}</p>
    </template>
  </div>
</template>
