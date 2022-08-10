<template>
  <div>
    <b-container>
      <h1 v-t="'messages.title'" />
      <div class="my-2">
        <icon-button class="mx-1" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      </div>
      <b-row>
        <search-field class="col" v-model="search" />
        <realm-select class="col" v-model="realm" />
        <template-select class="col" :realm="realm" v-model="template" />
      </b-row>
      <b-row>
        <status-select class="col" v-model="status">
          <template #after>
            <b-form-checkbox id="isDemo" v-model="isDemo">{{ $t('messages.demo.label') }}</b-form-checkbox>
          </template>
        </status-select>
        <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
        <count-select class="col" v-model="count" />
      </b-row>
      <p v-if="!messages.length" v-t="'messages.empty'" />
    </b-container>
    <b-container v-if="messages.length" fluid>
      <table id="table" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'messages.subject'" />
            <th scope="col" v-t="'messages.recipients'" />
            <th scope="col" v-t="'messages.sender.label'" />
            <th scope="col" v-t="'templates.select.label'" />
            <th scope="col" v-t="'messages.status.label'" />
            <th scope="col" v-t="'messages.updatedAt'" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="message in messages" :key="message.id">
            <td>
              <b-link :href="`/messages/${message.id}`">{{ message.subject }}</b-link>
              &nbsp;
              <b-badge v-if="message.isDemo" variant="info">{{ $t('messages.demo.label') }}</b-badge>
            </td>
            <td v-text="message.recipients" />
            <td>
              <template v-if="message.senderDisplayName">{{ message.senderDisplayName }} &lt;{{ message.senderAddress }}&gt;</template>
              <template v-else>{{ message.senderAddress }}</template>
            </td>
            <td>
              <template v-if="message.templateDisplayName">{{ message.templateDisplayName }} ({{ message.templateKey }})</template>
              <template v-else>{{ message.templateKey }}</template>
            </td>
            <td>
              <status-badge :message="message" />
            </td>
            <updated-cell :actor="message.updatedBy" :date="message.updatedAt" />
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="table" />
    </b-container>
  </div>
</template>

<script>
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import StatusBadge from './StatusBadge.vue'
import StatusSelect from './StatusSelect.vue'
import TemplateSelect from '@/components/Templates/TemplateSelect.vue'
import { getMessages } from '@/api/messages'

export default {
  name: 'MessageList',
  components: {
    RealmSelect,
    StatusBadge,
    StatusSelect,
    TemplateSelect
  },
  data() {
    return {
      count: 10,
      desc: true,
      isDemo: false,
      loading: false,
      messages: [],
      page: 1,
      realm: null,
      search: null,
      sort: 'UpdatedAt',
      status: null,
      template: null,
      total: 0
    }
  },
  computed: {
    hasErrors() {
      return this.status === null ? null : this.status === 'Failed'
    },
    params() {
      return {
        hasErrors: this.hasErrors,
        isDemo: this.isDemo,
        realm: this.realm,
        search: this.search,
        succeeded: this.succeeded,
        template: this.template,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return this.orderBy(
        Object.entries(this.$i18n.t('messages.sort.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    },
    succeeded() {
      return this.status === null ? null : this.status === 'Sent'
    }
  },
  methods: {
    async refresh(params = null) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await getMessages(params ?? this.params)
          this.messages = data.items
          this.total = data.total
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    params: {
      deep: true,
      immediate: true,
      async handler(newValue, oldValue) {
        if (
          newValue?.index &&
          oldValue &&
          (newValue.hasErrors !== oldValue.hasErrors ||
            newValue.isDemo !== oldValue.isDemo ||
            newValue.realm !== oldValue.realm ||
            newValue.search !== oldValue.search ||
            newValue.succeeded !== oldValue.succeeded ||
            newValue.template !== oldValue.template ||
            newValue.count !== oldValue.count)
        ) {
          this.page = 1
          await this.refresh()
        } else {
          await this.refresh(newValue)
        }
      }
    }
  }
}
</script>
