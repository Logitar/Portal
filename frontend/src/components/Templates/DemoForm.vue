<template>
  <div>
    <b-alert v-if="message" dismissible :variant="resultVariant" v-model="showResult">
      <strong>{{ $t('templates.statusFormat', { status }) }}</strong>
      {{ $t(`templates.status.${status}`) }}
      <b-link :href="`/messages/${message.id}`" target="_blank">{{ $t('templates.result.viewDetail') }} <font-awesome-icon icon="external-link-alt" /></b-link>
    </b-alert>
    <p>
      <i>
        {{ $t('templates.senderFormat') }}
        <b-link v-if="sender.displayName" :href="`/senders/${sender.id}`" target="_blank">
          {{ sender.displayName }} &lt;{{ sender.emailAddress }}&gt; <font-awesome-icon icon="external-link-alt"
        /></b-link>
        <b-link v-else :href="`/senders/${sender.id}`" target="_blank">{{ sender.emailAddress }} <font-awesome-icon icon="external-link-alt" /></b-link>.
      </i>
    </p>
    <locale-select v-model="locale" />
    <icon-button :disabled="loading" icon="paper-plane" :loading="loading" text="templates.sendToMe" variant="primary" @click="sendDemo" />
    <h3 v-t="'templates.variables.label'" />
    <div class="my-2">
      <icon-button icon="plus" text="templates.variables.add" variant="success" @click="addVariable" />
    </div>
    <validation-observer ref="variables">
      <b-row v-for="(variable, index) in variables" :key="index">
        <form-field
          class="col"
          hideLabel
          :id="`key_${index}`"
          label="templates.variables.key"
          :maxLength="255"
          placeholder="templates.variables.key"
          required
          :rules="{ identifier: true }"
          :value="variable.key"
          @input="setVariable(index, $event, variable.value)"
        />
        <form-field
          class="col"
          hideLabel
          :id="`value_${index}`"
          label="templates.variables.value"
          placeholder="templates.variables.value"
          required
          :value="variable.value"
          @input="setVariable(index, variable.key, $event)"
        >
          <icon-button class="mx-3" icon="times" variant="danger" @click="removeVariable(index)" />
        </form-field>
      </b-row>
    </validation-observer>
  </div>
</template>

<script>
import Vue from 'vue'
import { sendDemoMessage } from '@/api/messages'

export default {
  name: 'DemoForm',
  props: {
    sender: {
      type: Object,
      required: true
    },
    template: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      loading: false,
      locale: null,
      message: null,
      showResult: false,
      variables: []
    }
  },
  computed: {
    payload() {
      return {
        templateId: this.template.id,
        locale: this.locale,
        variables: [...this.variables]
      }
    },
    resultVariant() {
      switch (this.status) {
        case 'Failed':
          return 'danger'
        case 'Sent':
          return 'success'
        case 'Unsent':
          return 'warning'
      }
      return ''
    },
    status() {
      return this.message.hasSucceeded ? 'Sent' : this.message.hasErrors ? 'Failed' : 'Unsent'
    }
  },
  methods: {
    addVariable() {
      this.variables.push({ key: null, value: null })
    },
    removeVariable(index) {
      Vue.delete(this.variables, index)
    },
    setVariable(index, key, value) {
      Vue.set(this.variables, index, { key, value })
    },
    async sendDemo() {
      if (!this.loading) {
        this.loading = true
        this.showResult = false
        try {
          if (await this.$refs.variables.validate()) {
            const { data } = await sendDemoMessage(this.payload)
            this.message = data
            this.showResult = true
            this.$refs.variables.reset()
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
