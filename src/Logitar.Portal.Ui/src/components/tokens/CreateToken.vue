<script setup lang="ts">
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import ClaimList from "./ClaimList.vue";
import EmailAddressInput from "@/components/users/EmailAddressInput.vue";
import FormInput from "../shared/FormInput.vue";
import RealmSelect from "@/components/realms/RealmSelect.vue";
import type { Claim, CreatedToken } from "@/types/tokens";
import type { Realm } from "@/types/realms";
import { createToken } from "@/api/tokens";

const { t } = useI18n();

const audience = ref<string>("");
const claims = ref<Claim[]>([]);
const clipboardRef = ref<InstanceType<typeof FormInput> | null>(null);
const createdToken = ref<CreatedToken>();
const emailAddress = ref<string>("");
const isConsumable = ref<boolean>(false);
const issuer = ref<string>("");
const lifetime = ref<number>(0);
const realm = ref<Realm>();
const secret = ref<string>("");
const subject = ref<string>("");
const type = ref<string>("");

const emit = defineEmits<{
  (e: "error", value: unknown): void;
}>();

function copyToClipboard(): void {
  if (clipboardRef.value && createdToken.value) {
    clipboardRef.value.focus();
    navigator.clipboard.writeText(createdToken.value.token);
  }
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    createdToken.value = await createToken({
      isConsumable: isConsumable.value,
      realm: realm.value?.id,
      audience: audience.value,
      issuer: issuer.value,
      lifetime: lifetime.value,
      secret: secret.value,
      type: type.value,
      subject: subject.value,
      emailAddress: emailAddress.value,
      claims: claims.value,
    });
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <div>
    <form @submit.prevent="onSubmit">
      <div class="mb-3">
        <icon-submit :disabled="isSubmitting" icon="fas fa-id-card" :loading="isSubmitting" text="actions.create" />
      </div>
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
      <form-checkbox class="mb-2" id="isConsumable" label="tokens.isConsumable" v-model="isConsumable" />
      <div class="row">
        <FormInput
          class="col-lg-6"
          id="lifetime"
          label="tokens.lifetime"
          :min-value="0"
          :model-value="lifetime.toString()"
          type="number"
          @update:model-value="lifetime = Number($event)"
        />
        <FormInput class="col-lg-6" id="type" label="tokens.type.label" placeholder="tokens.type.placeholder" v-model="type" />
      </div>
      <div class="row">
        <FormInput class="col-lg-6" id="audience" label="tokens.audience.label" placeholder="tokens.audience.placeholder" v-model="audience" />
        <FormInput class="col-lg-6" id="issuer" label="tokens.issuer.label" placeholder="tokens.issuer.placeholder" v-model="issuer" />
      </div>
      <div class="row">
        <EmailAddressInput class="col-lg-6" placeholder="tokens.emailAddressPlaceholder" validate v-model="emailAddress" />
        <FormInput class="col-lg-6" id="subject" label="tokens.subject.label" placeholder="tokens.subject.placeholder" v-model="subject" />
      </div>
      <h3>{{ t("tokens.claims.title") }}</h3>
      <ClaimList v-model="claims" />
    </form>
    <template v-if="createdToken">
      <h3>{{ t("tokens.created.title") }}</h3>
      <app-alert show variant="success">
        <strong>{{ t("tokens.created.heading") }}</strong>
        <br />
        <FormInput id="clipboard" :model-value="createdToken.token" readonly ref="clipboardRef" @focus="$event.target.select()">
          <template #append>
            <icon-button icon="fas fa-clipboard" text="actions.clipboard" variant="warning" @click="copyToClipboard()" />
          </template>
        </FormInput>
        <a href="https://jwt.io/" target="_blank">{{ t("tokens.created.decoder") }} <font-awesome-icon icon="fas fa-arrow-up-right-from-square" /></a>
      </app-alert>
    </template>
  </div>
</template>
