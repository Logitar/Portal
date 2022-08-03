<template>
  <b-tab :title="$t('tokens.create')">
    <validation-observer ref="form">
      <b-form @submit.prevent="submit">
        <div class="my-2">
          <icon-submit :disabled="!hasChanges || loading" icon="id-card" :loading="loading" text="tokens.create" variant="primary" />
        </div>
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
          <form-field class="col" id="lifetime" label="tokens.lifetime" :minValue="0" type="number" v-model.number="lifetime" />
        </b-row>
        <b-row>
          <email-field class="col" validate v-model="email" />
          <form-field class="col" id="subject" label="tokens.subject.label" placeholder="tokens.subject.placeholder" v-model="subject" />
        </b-row>
        <h3 v-t="'tokens.claims.label'" />
        <div class="my-2">
          <icon-button icon="plus" text="tokens.claims.add" variant="success" @click="addClaim" />
        </div>
        <b-row v-for="(claim, index) in claims" :key="index">
          <form-field
            class="col"
            hideLabel
            :id="`type_${index}`"
            label="tokens.claims.type"
            :maxLength="256"
            placeholder="tokens.claims.type"
            required
            :value="claim.type"
            @input="setClaim(index, $event, claim.value)"
          />
          <form-field
            class="col"
            hideLabel
            :id="`value_${index}`"
            label="tokens.claims.value"
            placeholder="tokens.claims.value"
            required
            :value="claim.value"
            @input="setClaim(index, claim.type, $event)"
          >
            <icon-button class="mx-3" icon="times" variant="danger" @click="removeClaim(index)" />
          </form-field>
        </b-row>
        <template v-if="token">
          <h3 v-t="'tokens.token.label'" />
          <b-alert show variant="success">
            <strong v-t="'tokens.token.heading'" />
            <br />
            <b-input-group>
              <b-form-input readonly ref="input" :value="token" @focus="$event.target.select()" />
              <template #append>
                <icon-button icon="clipboard" text="tokens.token.clipboard" variant="warning" @click="copyToClipboard()" />
              </template>
            </b-input-group>
            <b-link href="https://jwt.io/" target="_blank">{{ $t('tokens.token.decoder') }} <font-awesome-icon icon="external-link-alt" /></b-link>
          </b-alert>
        </template>
      </b-form>
    </validation-observer>
  </b-tab>
</template>

<script>
import Vue from 'vue'
import EmailField from '@/components/User/EmailField.vue'
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import { createToken } from '@/api/tokens'

export default {
  name: 'CreateTokenTab',
  components: {
    EmailField,
    RealmSelect
  },
  data() {
    return {
      claims: [],
      email: null,
      lifetime: 0,
      loading: false,
      purpose: null,
      realm: null,
      subject: null,
      token: null
    }
  },
  computed: {
    hasChanges() {
      return this.realm || this.purpose || this.lifetime || this.email || this.subject || this.claims.length
    },
    payload() {
      return {
        lifetime: this.lifetime || null,
        purpose: this.purpose || null,
        realm: this.realm,
        email: this.email || null,
        subject: this.subject || null,
        claims: [...this.claims]
      }
    }
  },
  methods: {
    addClaim() {
      this.claims.push({ type: null, value: null })
    },
    copyToClipboard() {
      this.$refs.input.focus()
      navigator.clipboard.writeText(this.token)
    },
    removeClaim(index) {
      Vue.delete(this.claims, index)
    },
    setClaim(index, type, value) {
      Vue.set(this.claims, index, { type, value })
    },
    async submit() {
      if (!this.loading) {
        this.loading = true
        try {
          if (await this.$refs.form.validate()) {
            alert('FLAG')
            const { data } = await createToken(this.payload)
            this.token = data.token
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
