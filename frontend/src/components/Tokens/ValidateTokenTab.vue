<template>
  <b-tab :title="$t('tokens.validate')">
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit :disabled="!hasChanges || loading" icon="id-card" :loading="loading" text="tokens.validate" variant="primary" />
        </div>
        <form-field id="token" label="tokens.token.label" placeholder="tokens.token.placeholder" required v-model="token" />
        <b-row>
          <realm-select class="col" v-model="realm" />
          <form-field
            class="col"
            id="purpose"
            label="tokens.purpose.label"
            :maxLength="100"
            placeholder="tokens.purpose.placeholder"
            :rules="{ purpose: true }"
            v-model="purpose"
          />
        </b-row>
      </b-form>
    </validation-observer>
    <template v-if="result">
      <h3 v-t="'tokens.result.label'" />
      <template v-if="result.succeeded">
        <b-row>
          <email-field class="col" disabled :value="result.email" />
          <form-field class="col" disabled id="subject" label="tokens.subject.label" placeholder="tokens.subject.placeholder" :value="result.subject" />
        </b-row>
        <template v-if="result.claims && result.claims.length">
          <h5 v-t="'tokens.claims.label'" />
          <table class="table table-striped">
            <thead>
              <th scope="col" v-t="'tokens.claims.type'" />
              <th scope="col" v-t="'tokens.claims.value'" />
              <th scope="col" v-t="'tokens.claims.valueType'" />
            </thead>
            <tbody>
              <tr v-for="(claim, index) in result.claims" :key="index">
                <td v-text="claim.type" />
                <td v-if="format(claim.valueType) === 'integer' && claim.value.length === 10">
                  <span v-b-tooltip.hover :title="$d(new Date(Number(claim.value) * 1000), 'medium')">
                    <font-awesome-icon icon="info-circle" /> {{ claim.value }}
                  </span>
                </td>
                <td v-else v-text="claim.value" />
                <td v-text="format(claim.valueType)" />
              </tr>
            </tbody>
          </table>
        </template>
      </template>
      <template v-else>
        <h5 v-t="'tokens.result.errors'" />
        <table class="table table-striped">
          <thead>
            <tr>
              <th scope="col" v-t="'tokens.error.code'" />
              <th scope="col" v-t="'tokens.error.description'" />
            </tr>
          </thead>
          <tbody>
            <tr v-for="(error, index) in result.errors" :key="index">
              <td v-text="error.code" />
              <td v-text="error.description" />
            </tr>
          </tbody>
        </table>
      </template>
    </template>
  </b-tab>
</template>

<script>
import EmailField from '@/components/User/EmailField.vue'
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import { validateToken } from '@/api/tokens'

export default {
  name: 'ValidateTokenTab',
  components: {
    EmailField,
    RealmSelect
  },
  data() {
    return {
      loading: false,
      purpose: null,
      realm: null,
      result: null,
      token: null
    }
  },
  computed: {
    hasChanges() {
      return this.token || this.realm || this.purpose
    },
    payload() {
      return {
        token: this.token,
        purpose: this.purpose,
        realm: this.realm
      }
    }
  },
  methods: {
    format(valueType) {
      const index = valueType.indexOf('#')
      return index < 0 ? valueType : valueType.substring(index + 1)
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            const { data } = await validateToken(this.payload)
            this.result = data
            this.$refs.form.reset()
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  }
}
</script>
